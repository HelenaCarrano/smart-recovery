import type { DeclineReason, PaymentStatus } from './payment'

export interface PaymentsFiltersState {
  status: PaymentStatus | 'All'
  declineReason: DeclineReason | 'All'
  dateFrom: string
  dateTo: string
}

export const DEFAULT_PAYMENTS_FILTERS: PaymentsFiltersState = {
  status: 'All',
  declineReason: 'All',
  dateFrom: '',
  dateTo: '',
}
