import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import { getFormSubmissionById, type FormSubmissionDetailDto } from '../../api/cos-api';
import './submission-detail.scss';

export function SubmissionDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<FormSubmissionDetailDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getFormSubmissionById(Number(id))
      .then(setData)
      .catch((e) => console.error('Load error:', e))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <LoadPanel visible />;
  if (!data) return <div><h2>Submission not found</h2></div>;

  let batterySlots: Record<string, string>[] = [];
  try {
    if (data.batterySlotsJson) {
      batterySlots = JSON.parse(data.batterySlotsJson);
    }
  } catch { /* ignore */ }

  let problems: Record<string, string>[] = [];
  try {
    problems = data.problems.map((p) => {
      try { return JSON.parse(p.valuesJson); }
      catch { return { raw: p.valuesJson }; }
    });
  } catch { /* ignore */ }

  return (
    <div className="submission-detail">
      <div className="detail-header">
        <h2>Submission #{data.id}</h2>
        <Button text="Back" icon="back" stylingMode="outlined" onClick={() => navigate('/admin/submissions')} />
      </div>

      <div className="info-cards">
        <InfoCard label="Form" value={data.form?.title ?? `Form #${data.formId}`} />
        <InfoCard label="Tanggal" value={data.tanggal} />
        <InfoCard label="Line" value={data.lineId != null ? String(data.lineId) : '-'} />
        <InfoCard label="Shift" value={data.shiftId != null ? String(data.shiftId) : '-'} />
        <InfoCard label="Operator" value={data.operatorName ?? data.operatorEmpId ?? '-'} />
        {data.leaderName && <InfoCard label="Leader" value={data.leaderName} />}
        {data.kasubsieName && <InfoCard label="Kasubsie" value={data.kasubsieName} />}
        {data.kasieName && <InfoCard label="Kasie" value={data.kasieName} />}
      </div>

      {batterySlots.length > 0 && (
        <div className="section">
          <h3>Battery Slots</h3>
          <table>
            <thead>
              <tr>
                {Object.keys(batterySlots[0]).map((key) => (
                  <th key={key}>{key}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {batterySlots.map((slot, i) => (
                <tr key={i}>
                  {Object.values(slot).map((val, j) => (
                    <td key={j}>{String(val ?? '')}</td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {data.checkValues.length > 0 && (
        <div className="section">
          <h3>Check Values</h3>
          <table>
            <thead>
              <tr>
                <th>Setting Key</th>
                <th>Value</th>
              </tr>
            </thead>
            <tbody>
              {data.checkValues.map((cv) => (
                <tr key={cv.id}>
                  <td>{cv.settingKey}</td>
                  <td>{cv.value}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {problems.length > 0 && (
        <div className="section">
          <h3>Problems</h3>
          <table>
            <thead>
              <tr>
                <th>#</th>
                {Object.keys(problems[0]).map((key) => (
                  <th key={key}>{key}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {problems.map((problem, i) => (
                <tr key={i}>
                  <td>{i + 1}</td>
                  {Object.values(problem).map((val, j) => (
                    <td key={j}>{String(val ?? '')}</td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {data.signatures.length > 0 && (
        <div className="section">
          <h3>Signatures</h3>
          <div className="signatures-grid">
            {data.signatures.map((sig) => (
              <div key={sig.id} className="signature-card">
                <div className="signature-label">{sig.roleKey}</div>
                {sig.signatureData ? (
                  <img src={sig.signatureData} alt={`Signature: ${sig.roleKey}`} />
                ) : (
                  <div style={{ color: '#999', fontStyle: 'italic' }}>No signature</div>
                )}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

function InfoCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="info-card">
      <div className="info-label">{label}</div>
      <div className="info-value">{value}</div>
    </div>
  );
}
