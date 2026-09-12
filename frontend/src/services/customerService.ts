import { api } from './api'
import type { Customer } from '@/types/customer'

export async function getCustomers(): Promise<Customer[]> {
  const { data } = await api.get<Customer[]>('/api/customers')
  return data
}

export async function getCustomerById(id: string): Promise<Customer> {
  const { data } = await api.get<Customer>(`/api/customers/${id}`)
  return data
}
