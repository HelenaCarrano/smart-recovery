import { api } from './api'
import type { Customer } from '@/types/customer'
import type { CustomerListItem } from '@/types/customerListItem'
import type { PagedResult } from '@/types/pagedResult'

export interface CustomerQueryParams {
  page: number
  pageSize: number
  search?: string
}

export async function getCustomers(params: CustomerQueryParams): Promise<PagedResult<CustomerListItem>> {
  const { data } = await api.get<PagedResult<CustomerListItem>>('/api/customers', { params })
  return data
}

export async function getCustomerById(id: string): Promise<Customer> {
  const { data } = await api.get<Customer>(`/api/customers/${id}`)
  return data
}
