import { getCustomers } from '@/services/customerService'
import { getPaymentsByCustomer } from '@/services/paymentService'
import { getSubscriptionsByCustomer } from '@/services/subscriptionService'
import type { CustomerListItem } from '@/types/customerListItem'
import { useAsyncData } from './useAsyncData'

async function fetchCustomers(): Promise<CustomerListItem[]> {
  const customers = await getCustomers()

  return Promise.all(
    customers.map(async (customer) => {
      const [subscriptions, payments] = await Promise.all([
        getSubscriptionsByCustomer(customer.id),
        getPaymentsByCustomer(customer.id),
      ])

      return {
        id: customer.id,
        name: customer.name,
        email: customer.email,
        isActive: customer.isActive,
        subscriptionsCount: subscriptions.length,
        paymentsCount: payments.length,
      }
    }),
  )
}

export function useCustomers() {
  return useAsyncData(fetchCustomers, [])
}
