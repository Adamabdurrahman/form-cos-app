import React, { useRef, useEffect, useCallback, useState } from 'react';
import Popup from 'devextreme-react/popup';
import Button from 'devextreme-react/button';
import './SignaturePad.scss';

interface SignaturePadProps {
  onChange?: (dataUrl: string | null) => void;
  label?: string;
  name?: string;
}

export default function SignaturePad({ onChange, label, name }: SignaturePadProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [popupVisible, setPopupVisible] = useState(false);
  const [drawing, setDrawing] = useState(false);
  const [signatureData, setSignatureData] = useState<string | null>(null);

  // Setup canvas context when popup opens
  useEffect(() => {
    if (!popupVisible) return;
    const timer = setTimeout(() => {
      const ctx = canvasRef.current?.getContext('2d');
      if (!ctx) return;
      ctx.lineWidth = 2;
      ctx.lineCap = 'round';
      ctx.lineJoin = 'round';
      ctx.strokeStyle = '#000';
    }, 50);
    return () => clearTimeout(timer);
  }, [popupVisible]);

  const getPos = useCallback(
    (e: React.MouseEvent | React.TouchEvent) => {
      const canvas = canvasRef.current!;
      const rect = canvas.getBoundingClientRect();
      const scaleX = canvas.width / rect.width;
      const scaleY = canvas.height / rect.height;
      if ('touches' in e) {
        return {
          x: (e.touches[0].clientX - rect.left) * scaleX,
          y: (e.touches[0].clientY - rect.top) * scaleY,
        };
      }
      return {
        x: ((e as React.MouseEvent).clientX - rect.left) * scaleX,
        y: ((e as React.MouseEvent).clientY - rect.top) * scaleY,
      };
    },
    []
  );

  const startDraw = useCallback(
    (e: React.MouseEvent | React.TouchEvent) => {
      e.preventDefault();
      const ctx = canvasRef.current?.getContext('2d');
      if (!ctx) return;
      setDrawing(true);
      const pos = getPos(e);
      ctx.beginPath();
      ctx.moveTo(pos.x, pos.y);
    },
    [getPos]
  );

  const draw = useCallback(
    (e: React.MouseEvent | React.TouchEvent) => {
      if (!drawing) return;
      e.preventDefault();
      const ctx = canvasRef.current?.getContext('2d');
      if (!ctx) return;
      const pos = getPos(e);
      ctx.lineTo(pos.x, pos.y);
      ctx.stroke();
    },
    [drawing, getPos]
  );

  const endDraw = useCallback(() => {
    setDrawing(false);
  }, []);

  const clearCanvas = useCallback(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
  }, []);

  const handleSave = useCallback(() => {
    if (canvasRef.current) {
      const dataUrl = canvasRef.current.toDataURL();
      setSignatureData(dataUrl);
      onChange?.(dataUrl);
    }
    setPopupVisible(false);
  }, [onChange]);

  const handleRemove = useCallback(() => {
    setSignatureData(null);
    onChange?.(null);
  }, [onChange]);

  return (
    <div className="signature-pad-wrapper">
      {label && <div className="signature-pad-label">{label}</div>}

      {/* Thumbnail / trigger area */}
      <div className="signature-thumb" onClick={() => setPopupVisible(true)} title="Klik untuk tanda tangan">
        {signatureData ? (
          <img src={signatureData} alt="Tanda tangan" className="signature-thumb-img" />
        ) : (
          <div className="signature-thumb-placeholder">
            <span className="thumb-text">Tanda Tangan</span>
          </div>
        )}
      </div>

      {signatureData && (
        <button className="signature-remove-btn" onClick={handleRemove} type="button" title="Hapus tanda tangan">
          ×
        </button>
      )}

      {name && <div className="signature-pad-name">{name}</div>}

      {/* Modal popup for signing */}
      <Popup
        visible={popupVisible}
        onHiding={() => setPopupVisible(false)}
        title={`Tanda Tangan — ${label ?? ''}`}
        width={520}
        height={360}
        showCloseButton={true}
        dragEnabled={false}
      >
        <div className="signature-modal-content">
          <div className="signature-canvas-area">
            <canvas
              ref={canvasRef}
              width={480}
              height={200}
              onMouseDown={startDraw}
              onMouseMove={draw}
              onMouseUp={endDraw}
              onMouseLeave={endDraw}
              onTouchStart={startDraw}
              onTouchMove={draw}
              onTouchEnd={endDraw}
            />
            <div className="signature-baseline" />
          </div>
          <div className="signature-hint">Tanda tangan di area di atas</div>
          <div className="signature-modal-actions">
            <Button text="Hapus" icon="trash" stylingMode="outlined" type="danger" onClick={clearCanvas} />
            <Button text="Simpan" icon="check" stylingMode="contained" type="success" onClick={handleSave} />
          </div>
        </div>
      </Popup>
    </div>
  );
}
