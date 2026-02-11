import React, { useRef, useEffect, useCallback, useState } from 'react';
import Popup from 'devextreme-react/popup';
import Button from 'devextreme-react/button';
import { getEmployeeSignature, saveEmployeeSignature } from '../../api/cos-api';
import './SignaturePad.scss';

interface SignaturePadProps {
  onChange?: (dataUrl: string | null) => void;
  label?: string;
  name?: string;
  /** Employee ID — used to load/save signature from DB */
  empId?: string | null;
  initialValue?: string | null;
}

export default function SignaturePad({ onChange, label, name, empId, initialValue }: SignaturePadProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [popupVisible, setPopupVisible] = useState(false);
  const [drawing, setDrawing] = useState(false);
  const [mode, setMode] = useState<'draw' | 'upload'>('draw');
  const [uploadPreview, setUploadPreview] = useState<string | null>(null);

  const [signatureData, setSignatureData] = useState<string | null>(initialValue ?? null);

  // Track whether we already loaded from DB for this empId
  const loadedEmpIdRef = useRef<string | null>(null);

  // Load signature from DB when empId changes (and no server initialValue)
  useEffect(() => {
    if (!empId || empId === loadedEmpIdRef.current) return;
    loadedEmpIdRef.current = empId;

    // If there's an initialValue from server (edit mode), use that instead
    if (initialValue) {
      setSignatureData(initialValue);
      return;
    }

    getEmployeeSignature(empId)
      .then(result => {
        if (result.signatureData) {
          setSignatureData(result.signatureData);
          onChange?.(result.signatureData);
        }
      })
      .catch(() => {
        // silently ignore — no saved signature
      });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [empId]);

  // Sync when initialValue prop changes from outside (edit mode loading)
  useEffect(() => {
    if (initialValue !== undefined && initialValue !== null) {
      setSignatureData(initialValue);
    }
  }, [initialValue]);

  // Setup canvas when popup opens
  useEffect(() => {
    if (!popupVisible) return;
    setMode('draw');
    setUploadPreview(null);
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

  /* ---------- drawing ---------- */
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

  const endDraw = useCallback(() => setDrawing(false), []);

  const clearCanvas = useCallback(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
  }, []);

  /* ---------- upload ---------- */
  const handleFileChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      setUploadPreview(result);
    };
    reader.readAsDataURL(file);
    e.target.value = '';
  }, []);

  /* ---------- save / remove ---------- */
  const handleSave = useCallback(() => {
    let dataUrl: string | null = null;

    if (mode === 'draw') {
      if (canvasRef.current) {
        dataUrl = canvasRef.current.toDataURL();
      }
    } else {
      dataUrl = uploadPreview;
    }

    if (dataUrl) {
      setSignatureData(dataUrl);
      onChange?.(dataUrl);

      // Persist to DB if we have an empId
      if (empId) {
        saveEmployeeSignature(empId, dataUrl).catch(() => {
          // silently ignore save errors
        });
      }
    }
    setPopupVisible(false);
  }, [mode, uploadPreview, empId, onChange]);

  const handleRemove = useCallback(() => {
    setSignatureData(null);
    onChange?.(null);
    // Note: we do NOT delete from DB on remove — the saved signature stays
    // so next time the employee signs a new form, it auto-loads again
  }, [onChange]);

  return (
    <div className="signature-pad-wrapper">
      {label && <div className="signature-pad-label">{label}</div>}

      {/* Thumbnail */}
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

      {/* Hidden file input */}
      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        style={{ display: 'none' }}
        onChange={handleFileChange}
      />

      {/* Popup */}
      <Popup
        visible={popupVisible}
        onHiding={() => setPopupVisible(false)}
        title={`Tanda Tangan — ${label ?? ''}`}
        width={520}
        height={420}
        showCloseButton={true}
        dragEnabled={false}
      >
        <div className="signature-modal-content">
          {/* Mode toggle */}
          <div className="signature-mode-toggle">
            <button
              type="button"
              className={`sig-tab ${mode === 'draw' ? 'active' : ''}`}
              onClick={() => setMode('draw')}
            >
              Gambar
            </button>
            <button
              type="button"
              className={`sig-tab ${mode === 'upload' ? 'active' : ''}`}
              onClick={() => setMode('upload')}
            >
              Upload
            </button>
          </div>

          {/* Draw mode */}
          {mode === 'draw' && (
            <>
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
            </>
          )}

          {/* Upload mode */}
          {mode === 'upload' && (
            <>
              <div className="signature-upload-area">
                {uploadPreview ? (
                  <div className="upload-preview-wrap">
                    <img src={uploadPreview} alt="Preview" className="upload-preview-img" />
                    <button
                      type="button"
                      className="upload-change-btn"
                      onClick={() => fileInputRef.current?.click()}
                    >
                      Ganti Gambar
                    </button>
                  </div>
                ) : (
                  <div className="upload-dropzone" onClick={() => fileInputRef.current?.click()}>
                    <span className="upload-text">Klik untuk pilih gambar tanda tangan</span>
                    <span className="upload-subtext">PNG, JPG, atau format gambar lainnya</span>
                  </div>
                )}
              </div>
              <div className="signature-modal-actions">
                <Button
                  text="Simpan"
                  icon="check"
                  stylingMode="contained"
                  type="success"
                  onClick={handleSave}
                  disabled={!uploadPreview}
                />
              </div>
            </>
          )}
        </div>
      </Popup>
    </div>
  );
}
