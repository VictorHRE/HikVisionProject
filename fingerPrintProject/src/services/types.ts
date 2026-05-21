export type Employee = {
  id: string | number
  identification?: string
  name?: string
  lastName?: string
  position?: string
  phone?: string
  email?: string
  active: boolean
  idStoreHQ?: number
  status?: string
  userType?: string
  gender?: string | null
  beginTime?: string
  endTime?: string
  birthDate?: string
  createdAt?: string
  finger?: string
}

// `Branch` was replaced by `ContiendaStore` — stores may come from Contienda API
export type ContiendaStore = {
  id: string | number
  idStoreHQ?: number
  // `name` kept for compatibility with older code; servers may use `storeName`.
  name?: string
  storeName?: string
  active?: string | boolean
}

// Catalog option used by getCatalog endpoints (label shown to user, value is stored)
export type CatalogOption = {
  value: string
  label: string
}

// (old duplicate removed) 

/*
export type HistoryEntry = {
  id: string
  employeeId: string
  branchId: string
  type: 'in' | 'out'
  ts: string // ISO
}

// Catalog option used by getCatalog endpoints
export type CatalogOption = {
  value: string
  label: string
}

HistoryEntry type commented out — deshabilitado por petición del usuario.
*/

// Authentication helpers
export type LoginRequest = {
  id: number
  name: string
  email?: string
  password: string
}

export type LoginResponse = {
  isSuccess: boolean
  message?: string
  token?: string
  user?: unknown
}
