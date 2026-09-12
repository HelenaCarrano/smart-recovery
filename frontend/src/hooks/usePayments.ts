import { getPayments } from '@/services/paymentService'
import { getRecoveryAnalysisByPayment } from '@/services/recoveryService'
import type { PagedResult } from '@/types/pagedResult'
import type { PaymentListItem } from '@/types/paymentListItem'
import type { PaymentsFiltersState } from '@/types/paymentsFilters'
import { useAsyncData } from './useAsyncData'

/**
 * GET /api/payments já traz customerName resolvido e pagina/filtra no servidor — nada de
 * composição N+1 aqui. O Recovery Score/ação só existem para pagamentos recusados e não vêm
 * no endpoint de listagem, então enriquecemos apenas os itens Declined da página atual
 * (no máximo `pageSize` chamadas extras, não uma por pagamento do banco inteiro).
 */
async function fetchPayments(page: number, pageSize: number, filters: PaymentsFiltersState): Promise<PagedResult<PaymentListItem>> {
  const result = await getPayments({
    page,
    pageSize,
    status: filters.status === 'All' ? undefined : filters.status,
    declineReason: filters.declineReason === 'All' ? undefined : filters.declineReason,
    dateFrom: filters.dateFrom || undefined,
    dateTo: filters.dateTo || undefined,
  })

  const items = await Promise.all(
    result.items.map(async (item) => {
      if (item.status !== 'Declined') return item

      const analysis = await getRecoveryAnalysisByPayment(item.id)
      return {
        ...item,
        recoveryScore: analysis?.recoveryScore ?? null,
        recommendedAction: analysis?.recommendedAction ?? null,
      }
    }),
  )

  return { ...result, items }
}

export function usePayments(page: number, pageSize: number, filters: PaymentsFiltersState) {
  return useAsyncData(
    () => fetchPayments(page, pageSize, filters),
    [page, pageSize, filters.status, filters.declineReason, filters.dateFrom, filters.dateTo],
  )
}
