/** Espelha o JSON retornado por GET /api/customers e GET /api/customers/{id}. */
export interface Customer {
  id: string
  name: string
  email: string
  document: string
  isActive: boolean
  createdAt: string
}
