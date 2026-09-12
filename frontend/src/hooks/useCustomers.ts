import { getCustomers } from '@/services/customerService'
import { useAsyncData } from './useAsyncData'

/** GET /api/customers já pagina, filtra por busca e traz as contagens — nenhuma composição necessária aqui. */
export function useCustomers(page: number, pageSize: number, search: string) {
  return useAsyncData(
    () => getCustomers({ page, pageSize, search: search || undefined }),
    [page, pageSize, search],
  )
}
