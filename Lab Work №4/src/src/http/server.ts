import express from 'express';
import { CumListService } from '../application/cumListService';

const app = express();
const service = new CumListService();

app.use(express.json());

app.get('/api/cumlists', (req, res) => {
  res.json(service.list(req.query as Record<string, string | undefined>));
});

app.get('/api/cumlists/:id', (req, res) => {
  const item = service.getById(Number(req.params.id));
  if (!item) {
    return res.status(404).json({ errorCode: 'DOCUMENT_NOT_FOUND', message: 'CumList not found', details: {} });
  }
  return res.json(item);
});

app.get('/api/cumlists/:id/dues', (req, res) => {
  res.json(service.getDues(Number(req.params.id)));
});

app.get('/api/cumlists/:id/history', (req, res) => {
  res.json(service.getHistory(Number(req.params.id)));
});

app.get('/api/lookups/rejection-reasons', (_req, res) => {
  res.json(service.getRejectionReasons());
});

app.post('/api/cumlists/:id/sign', (req, res) => {
  const userId = Number(req.header('X-User-Id') ?? 0);
  const result = service.sign(Number(req.params.id), userId);
  if (!result) {
    return res.status(404).json({ errorCode: 'DOCUMENT_NOT_FOUND', message: 'CumList not found', details: {} });
  }
  return res.status(202).json(result);
});

app.post('/api/cumlists/:id/reject', (req, res) => {
  const userId = Number(req.header('X-User-Id') ?? 0);
  const result = service.reject(Number(req.params.id), userId);
  if (!result) {
    return res.status(404).json({ errorCode: 'DOCUMENT_NOT_FOUND', message: 'CumList not found', details: {} });
  }
  return res.status(202).json(result);
});

app.post('/api/cumlists/operations/bulk-sign', (req, res) => {
  return res.status(202).json(service.bulkSign(req.body.documentIds ?? []));
});

app.put('/api/cumlists/:id/lock', (req, res) => {
  const result = service.lock(Number(req.params.id), req.body.owner ?? 'unknown');
  if (!result) {
    return res.status(404).json({ errorCode: 'DOCUMENT_NOT_FOUND', message: 'CumList not found', details: {} });
  }
  return res.json(result);
});

app.delete('/api/cumlists/:id/lock', (req, res) => {
  const result = service.unlock(Number(req.params.id));
  if (!result) {
    return res.status(404).json({ errorCode: 'DOCUMENT_NOT_FOUND', message: 'CumList not found', details: {} });
  }
  return res.json(result);
});

app.listen(3000, () => {
  // eslint-disable-next-line no-console
  console.log('CumList API started on http://localhost:3000');
});
