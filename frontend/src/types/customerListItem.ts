/**
 * View model da tabela de Clientes. GET /api/customers não traz contagem de
 * assinaturas/pagamentos — combinamos com /api/subscriptions/by-customer e
 * /api/payments/by-customer (mesmo padrão já usado na listagem de Pagamentos).
 */
export interface CustomerListItem {
  id: string
  name: string
  email: string
  isActive: boolean
  subscriptionsCount: number
  paymentsCount: number
}
