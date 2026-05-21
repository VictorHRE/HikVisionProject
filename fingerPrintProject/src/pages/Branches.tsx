import { useEffect, useState } from 'react';
import * as Api from '../services/api';
import type { ContiendaStore } from '../services/types';
import './Employees.css'; // Reutilizamos estilos
import './Branches.css';

export default function BranchesPage() {
  const [contienda, setContienda] = useState<ContiendaStore[]>([])
  const [searchTerm, setSearchTerm] = useState('')
  const [page, setPage] = useState(1)
  const pageSize = 10 // Filtered results
  const filtered = contienda.filter(c => {
    const q = searchTerm.trim().toLowerCase()
    if (!q) return true
    return String(c.storeName ?? c.name ?? c.id).toLowerCase().includes(q) || String(c.id).includes(q)
  })
  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize))
  const pageItems = filtered.slice((page - 1) * pageSize, page * pageSize)

  useEffect(() => { setPage(1) }, [searchTerm])
  useEffect(() => { if (page > totalPages) setPage(totalPages) }, [totalPages, page])

  useEffect(() => {
    Api.getContiendaStores().then(setContienda).catch(e => { console.error('API contienda error', e); setContienda([]) })
  }, []);


  return (
    <div className="page-container branches-page">
      <h2 className="page-header">Gestión de Sucursales</h2>
      {/* Buscador para tiendas Contienda (reemplaza la opción de agregar) */}
      <div className="add-form">
        <input
          placeholder="Buscar tienda por nombre..."
          value={searchTerm}
          onChange={e => setSearchTerm(e.target.value)}
          className="form-input"
        />
        {/* no limpiar, el usuario pidió sin botón limpiar */}
      </div>
      <div>
        <h3>Tiendas</h3>
        {contienda.length === 0 && <div>No hay tiendas</div>}
        <table className="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nombre de la tienda</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
        {pageItems.map((c, idx) => (
          <tr key={`${String(c.id)}-${idx}`}>
            <td>{c.id}</td>
            <td>{c.storeName ?? c.name ?? '-'}</td>
            <td>{'Activa'}</td>
          </tr>
        ))}
          </tbody>
        </table>
        {searchTerm && filtered.length === 0 && (
          <div>No se encontraron tiendas para "{searchTerm}"</div>
        )}
        {/* Pagination controls */}
        <div className="pagination-wrapper">
          {totalPages > 1 && (
            <div className="pagination">
              <button className="btn" disabled={page === 1} onClick={() => setPage(p => Math.max(1, p - 1))}>Previous</button>
              {Array.from({ length: totalPages }).map((_, i) => (
                <button key={i} className={`btn ${page === i + 1 ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setPage(i + 1)}>{i + 1}</button>
              ))}
              <button className="btn" disabled={page === totalPages} onClick={() => setPage(p => Math.min(totalPages, p + 1))}>Next</button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
