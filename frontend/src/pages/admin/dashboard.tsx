import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import { getFormDefinitions, getFormSubmissions } from '../../api/cos-api';

export function AdminDashboard() {
  const navigate = useNavigate();
  const [formCount, setFormCount] = useState(0);
  const [submissionCount, setSubmissionCount] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const [forms, submissions] = await Promise.all([
          getFormDefinitions(),
          getFormSubmissions(),
        ]);
        setFormCount(forms.length);
        setSubmissionCount(submissions.length);
      } catch (e) {
        console.error('Dashboard load error:', e);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  return (
    <div>
      <LoadPanel visible={loading} />
      <h2 style={{ marginTop: 0 }}>Welcome to the Admin Panel</h2>
      <p style={{ color: '#666', marginBottom: 24 }}>
        Manage form definitions, submissions, personnel, and battery types.
      </p>

      <div style={{ display: 'flex', gap: 20, flexWrap: 'wrap', marginBottom: 32 }}>
        <DashCard title="Total Forms" value={formCount} color="#4fc3f7" />
        <DashCard title="Total Submissions" value={submissionCount} color="#81c784" />
      </div>

      <h3>Quick Links</h3>
      <div style={{ display: 'flex', gap: 12 }}>
        <Button
          text="View Forms"
          icon="fields"
          type="default"
          stylingMode="contained"
          onClick={() => navigate('/admin/forms')}
        />
        <Button
          text="View Submissions"
          icon="doc"
          type="default"
          stylingMode="contained"
          onClick={() => navigate('/admin/submissions')}
        />
      </div>
    </div>
  );
}

function DashCard({ title, value, color }: { title: string; value: number; color: string }) {
  return (
    <div
      style={{
        background: '#fff',
        borderRadius: 8,
        padding: '24px 32px',
        minWidth: 200,
        boxShadow: '0 2px 8px rgba(0,0,0,0.08)',
        borderTop: `4px solid ${color}`,
      }}
    >
      <div style={{ fontSize: 14, color: '#888', marginBottom: 8 }}>{title}</div>
      <div style={{ fontSize: 32, fontWeight: 700, color: '#222' }}>{value}</div>
    </div>
  );
}
