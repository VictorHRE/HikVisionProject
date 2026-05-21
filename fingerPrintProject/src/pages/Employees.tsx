import { useEffect, useState } from 'react';
import * as Api from '../services/api';
import type { Employee, ContiendaStore } from '../services/types';
import './Employees.css';
import './Branches.css'; // reutilizamos estilos de paginación para consistencia

export default function EmployeesPage() {
  const [emps, setEmps] = useState<Employee[]>([]);
  const [searchIdentification, setSearchIdentification] = useState('')
  // Indicador de si la última búsqueda fue por identificación (endpoint)
  const [searchByIdentification, setSearchByIdentification] = useState(false)
  const [page, setPage] = useState(1)
  const pageSize = 10
  const [showModal, setShowModal] = useState(false)
  const [modalEmp, setModalEmp] = useState<Employee | null>(null)
  const [modalStoreId, setModalStoreId] = useState('')
  const [showUpdateModal, setShowUpdateModal] = useState(false)
  const [updateStatus, setUpdateStatus] = useState<string | undefined>(undefined)
  const [updateUserType, setUpdateUserType] = useState<string | undefined>(undefined)
  const [updateGender, setUpdateGender] = useState<string | undefined>(undefined)
  const [catalogStatus, setCatalogStatus] = useState<{value:string,label:string}[]>([])
  const [catalogTypes, setCatalogTypes] = useState<{value:string,label:string}[]>([])
  const [catalogGenders, setCatalogGenders] = useState<{value:string,label:string}[]>([])
  const [contiendaStores, setContiendaStores] = useState<ContiendaStore[]>([])

  useEffect(() => {
    Api.getEmployees().then(setEmps).catch(e => { console.error('API error', e); setEmps([]) })
    Api.getContiendaStores().then(setContiendaStores).catch(e => { console.error('API contienda error', e); setContiendaStores([]) })
  }, []);

  // Reset page when user changes the search field
  useEffect(() => { setPage(1) }, [searchIdentification])

  async function refresh() {
    setEmps(await Api.getEmployees())
    setSearchByIdentification(false)
  }

  async function handleSearchEmployee() {
    const q = searchIdentification.trim()
    if (!q) { // vacío: restaurar listado completo
      await refresh()
      return
    }
    try {
      const emp = await Api.getEmployeeById(q)
      if (emp) {
        setEmps([emp])
        setSearchByIdentification(true)
        setPage(1)
      } else {
        // Si no se encontró, dejar vacío para que el usuario sepa que no hay coincidencia
        setEmps([])
        setSearchByIdentification(true)
      }
    } catch (e) {
      console.error('getEmployeeById failed', e)
      alert('No se pudo buscar el empleado')
    }
  }

  async function handleAddFingerprint(emp: Employee) {
    if ((emp.finger && emp.finger === 'Asociado') ) {
      console.debug('addFingerprint: already has fingerprint')
      return
    }
    try {
      const payload: Employee = {
        ...emp,
        name: emp.name ?? `${emp.name ?? ''}`.trim(),
      }
      const result = await Api.addFingerprintedEmployee(payload) as unknown as { success?: boolean; message?: string }
      if (!result || result.success === false) {

                    const typed = result as unknown as { message?: string; Message?: string; msg?: string }
      const backendMsg = typed?.message ?? typed?.Message ?? typed?.msg
      if (backendMsg) {
        alert(String(backendMsg))
      } else {
        alert('Huella registrada correctamente')
      }
    }
    refresh()
    } catch (err) {
      console.error('add fingerprint (device) failed', err)
      alert(err ?? 'Error al registrar huella')
    }
  }

  async function handleClearFingerprints(emp: Employee) {
    try {
      // Silent: remove all fingerprints without confirmations or alert
       const result = await Api.deleteFingerprintedEmployee(emp)as unknown as { statuscode?: number; message?: string }
        if (!result || result.statuscode !== 200) {
            const typed = result as unknown as { message?: string; Message?: string; msg?: string }
      const backendMsg = typed?.message ?? typed?.Message ?? typed?.msg
      if (backendMsg) {
        alert(String(backendMsg))
      } else {
        alert('Huella borrada correctamente')
      }
      }
      refresh()
    } catch (err) {
      console.error('clear fingerprints failed', err)
      alert(err ?? 'Error al borrar huella')
    }
  }

  async function handleAssociateToStore(emp: Employee) {
    setModalEmp(emp)
    setModalStoreId(emp.idStoreHQ ? String(emp.idStoreHQ) : '')
    setShowModal(true)
  }

  async function handleOpenUpdate(emp: Employee) {
    setModalEmp(emp)
    // preset values
    setUpdateStatus(emp.status ?? undefined)
    setUpdateUserType(emp.userType ?? undefined)
    setUpdateGender(emp.gender ?? undefined)

    // fetch catalog values in parallel
    try {
      const [sts, types, genders] = await Promise.all([
        Api.getCatalog('STATUS'),
        Api.getCatalog('TYPE'),
        Api.getCatalog('GENDER')
      ])
      // each item may be a string or an object — normalize to { value, label }
      const toOpts = (arr: unknown[]) => arr.map(x => {
        if (typeof x === 'string') return { value: x, label: x }
        if (typeof x === 'object' && x !== null) {
          const obj = x as Record<string, unknown>
          // Prefer codigo/id as 'value' (backend identifier), but show descripcion as label
          const value = String(obj['codigo'] ?? obj['id'] ?? obj['value'] ?? obj['descripcion'] ?? '')
          const label = String(obj['descripcion'] ?? obj['label'] ?? obj['value'] ?? obj['codigo'] ?? obj['id'] ?? '')
          return { value, label }
        }
        return { value: String(x), label: String(x) }
      })
      setCatalogStatus(toOpts(sts).sort((a,b) => a.label.localeCompare(b.label)))
      setCatalogTypes(toOpts(types).sort((a,b) => a.label.localeCompare(b.label)))
      setCatalogGenders(toOpts(genders).sort((a,b) => a.label.localeCompare(b.label)))
      // show modal only when catalogs are loaded to avoid render issues
      setShowUpdateModal(true)
    } catch (err) {
      console.error('getCatalog failed', err)
      alert('Error cargando datos del catálogo')
    }
  }

  async function handleModalConfirm() {
    if (!modalEmp) return
    const storeId = Number(modalStoreId)
    if (!modalStoreId || Number.isNaN(storeId)) { alert('ID inválido'); return }
    try {
      // create copy and set idStoreHQ
      const payload: Employee = { ...modalEmp, idStoreHQ: storeId }
      // If the employee already had an associated store, update; otherwise add
      const isUpdate = Boolean(modalEmp.idStoreHQ && modalEmp.idStoreHQ > 0)
      const res = isUpdate
        ? await Api.updateEmployeeToDevice(payload)
        : await Api.addEmployeeToDevice(payload)

      // Prefer backend message if available, otherwise show a friendly message
      const typed = res as unknown as { message?: string; Message?: string; msg?: string }
      const backendMsg = typed?.message ?? typed?.Message ?? typed?.msg
      if (backendMsg) {
        alert(String(backendMsg))
      } else {
        alert(isUpdate ? 'Tienda actualizada correctamente' : 'Empleado asociado correctamente')
      }
      setShowModal(false)
      setModalEmp(null)
      setModalStoreId('')
      refresh()
    } catch (err : unknown) {
      console.error(err)
      alert(err ?? 'Error en Asociar tienda')
    }
  }

  async function handleUpdateConfirm() {
    if (!modalEmp) return
    try {
      // Map selected catalog 'value' -> catalog 'label' before sending to backend.
      const statusLabel = catalogStatus.find(s => s.value === updateStatus)?.label ?? updateStatus
      const typeLabel = catalogTypes.find(s => s.value === updateUserType)?.label ?? updateUserType
      const genderLabel = catalogGenders.find(s => s.value === updateGender)?.label ?? updateGender

      const payload: Employee = { ...modalEmp, status: statusLabel, userType: typeLabel, gender: genderLabel }
      const res = await Api.updateEmployeeToDevice(payload)
      const typed = res as unknown as { message?: string; Message?: string; msg?: string }
      const backendMsg = typed?.message ?? typed?.Message ?? typed?.msg
      if (backendMsg) alert(String(backendMsg))
      else alert('Empleado actualizado correctamente')
      setShowUpdateModal(false)
      setModalEmp(null)
      refresh()
    } catch (err) {
      console.error(err)
      alert(err)
    }
  }

  // Filtered + pagination
  const filtered = emps.filter(emp => {
    const q = searchIdentification.trim().toLowerCase()
    if (!q) return true
    // Cuando se buscó por identificación vía endpoint ya limitamos a 1
    if (searchByIdentification) return true
    const identification = String(emp.identification ?? '').toLowerCase()
    return identification.includes(q)
  })

  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize))
  useEffect(() => {
    if (page > totalPages) setPage(totalPages)
  }, [page, totalPages])
  const pageItems = filtered.slice((page - 1) * pageSize, page * pageSize)

  return (
    <div className="page-container employees-page">
      <h2 className="page-header">Gestión de Empleados</h2>
      {/* Solo dejamos el buscador */}
      <div className="add-form">
        <input
          placeholder="Buscar empleado por identificación"
          value={searchIdentification}
          onChange={e => { setSearchIdentification(e.target.value); if (!e.target.value) { setSearchByIdentification(false) } }}
          className="form-input"
          style={{ width: 320 }}
        />
        <button onClick={handleSearchEmployee} className="btn btn-primary" style={{ marginLeft: 8 }}>Buscar</button>
        {searchByIdentification && searchIdentification.trim() && (
          <button onClick={refresh} className="btn btn-secondary" style={{ marginLeft: 8 }}>Limpiar</button>
        )}
      </div>
      <table className="data-table">
        <thead>
          <tr>
            <th>Identificación</th>
            <th>Nombre</th>
            <th>Apellido</th>
            <th>Tienda</th>
            <th>Estado</th>
            <th>Tipo de usuario</th>
            <th>Género</th>
            <th>Huella</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {pageItems.map((emp, idx) => {
            const store = contiendaStores.find(s => String(s.id) === String(emp.idStoreHQ ?? ''))
            return (
              <tr key={`${typeof emp.id === 'object' && emp.id ? JSON.stringify(emp.id) : String(emp.id)}-${idx}`}>
                <td>{emp.identification ?? '-'}</td>
                <td>{emp.name ?? '-'}</td>
                <td>{emp.lastName ?? '-'}</td>
                <td>
                  <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
                    <span style={{ minWidth: 160 }} className="store-name">{store ? (store.storeName ?? store.name ?? String(store.id)) : (emp.idStoreHQ ? String(emp.idStoreHQ) : '-')}</span>
                    {emp.idStoreHQ && emp.idStoreHQ > 0
                      ? <button onClick={() => handleAssociateToStore(emp)} className="btn btn-secondary btn-sm">Actualizar tienda</button>
                      : <button onClick={() => handleAssociateToStore(emp)} className="btn btn-primary btn-sm">Asociar</button>
                    }
                  </div>
                </td>
                <td>
                  <span className={emp.status === 'ACTIVE' ? 'status-active' : 'status-inactive'}>
                    {emp.status ?? '-'}
                  </span>
                </td>
                <td>{emp.userType ?? '-'}</td>
                <td>{emp.gender ?? '-'}</td>
                <td>
                  <div className="huella-status">
                    <span className={`huella ${emp.finger === 'Asociado' ? 'huella-associated' : 'huella-not-associated'}`}>
                      {emp.finger ?? '-'}
                    </span>
                  </div>
                </td>
                <td className="actions-cell">
                  <button
                    onClick={() => emp.finger === 'NOASOCIADO' ? handleAddFingerprint(emp) : handleClearFingerprints(emp)}
                    className={emp.finger === 'NOASOCIADO' ? 'btn btn-secondary' : 'btn btn-danger'}
                  >
                    {emp.finger === 'NOASOCIADO' ?'Agregar huella' : 'Eliminar huella'}
                  </button>
                  <button onClick={() => handleOpenUpdate(emp)} className="btn btn-secondary">Actualizar</button>
                  </td>
              </tr>
            )
          })}
        </tbody>
      </table>
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
      {showModal && modalEmp && (
        <div className="modal-backdrop">
          <div className="modal">
            <h3>Asociar empleado {modalEmp.name ?? modalEmp.name ?? modalEmp.id}</h3>
            
            <label htmlFor="select-sucursal">Seleccionar sucursal</label>
            <select
              id="select-sucursal"
              value={modalStoreId}
              onChange={e => setModalStoreId(e.target.value)}
              className="form-input"
            >
              <option value="">-- Selecciona una sucursal --</option>
              {contiendaStores.map(s => (
                <option key={String(s.idStoreHQ)} value={String(s.idStoreHQ)}>{s.storeName ?? s.name ?? s.id}</option>
              ))}
            </select>
            <div className="modal-actions">
              <button className="btn btn-primary" onClick={handleModalConfirm}>{modalEmp?.idStoreHQ ? 'Actualizar' : 'Asociar'}</button>
              <button className="btn btn-secondary" onClick={() => { setShowModal(false); setModalEmp(null); setModalStoreId('') }}>Cancelar</button>
            </div>
          </div>
        </div>
      )}
      {showUpdateModal && modalEmp && (
        <div className="modal-backdrop">
          <div className="modal">
            <h3>Actualizar empleado {modalEmp.name ?? modalEmp.id}</h3>

            <div className="form-grid">
              <label>Estado</label>
              <select value={updateStatus ?? ''} onChange={e => setUpdateStatus(e.target.value)} className="form-input">
              <option value="">-- Selecciona estado --</option>
              {(catalogStatus.length ? catalogStatus : [{value:'ACTIVE',label:'ACTIVE'},{value:'INACTIVE',label:'INACTIVE'},{value:'UNCLAIMED',label:'UNCLAIMED'}]).map(s => (
                <option key={s.value} value={s.value}>{s.label}</option>
              ))}
            </select>
              <label>Género</label>
              <select value={updateGender ?? ''} onChange={e => setUpdateGender(e.target.value)} className="form-input">
                <option value="">-- Selecciona género --</option>
                {(catalogGenders.length ? catalogGenders : [{value:'MALE',label:'MALE'},{value:'FEMALE',label:'FEMALE'},{value:'OTHER',label:'OTHER'},{value:'UNSPECIFIED',label:'UNSPECIFIED'}]).map(g => (
                  <option key={g.value} value={g.value}>{g.label}</option>
                ))}
              </select>

              <label>Tipo de usuario</label>
              <select value={updateUserType ?? ''} onChange={e => setUpdateUserType(e.target.value)} className="form-input">
                <option value="">-- Selecciona tipo --</option>
                {(catalogTypes.length ? catalogTypes : [{value:'ADMIN',label:'ADMIN'},{value:'USER',label:'USER'},{value:'GUEST',label:'GUEST'}]).map(t => (
                  <option key={t.value} value={t.value}>{t.label}</option>
                ))}
              </select>
            </div>

            <div className="modal-actions">
              <button className="btn btn-primary" onClick={handleUpdateConfirm}>Guardar</button>
              <button className="btn btn-secondary" onClick={() => { setShowUpdateModal(false); setModalEmp(null); }}>Cancelar</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
