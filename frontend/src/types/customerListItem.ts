/** Espelha CustomerListItemDto de GET /api/customers — contagens já vêm calculadas pelo backend. */
export interface CustomerListItem {
  id: string
  name: string
  email: string
  phone: string
  isActive: boolean
  subscriptionsCount: number
  paymentsCount: number
}
