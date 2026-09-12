import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { type DataTableColumn, DataTable } from '@/components/ui/DataTable'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { useCustomers } from '@/hooks/useCustomers'
import type { CustomerListItem } from '@/types/customerListItem'
import { useNavigate } from 'react-router-dom'

const columns: DataTableColumn<CustomerListItem>[] = [
  { key: 'name', header: 'Nome', render: (row) => <span className="font-medium text-slate-900">{row.name}</span> },
  { key: 'email', header: 'Email', render: (row) => row.email },
  { key: 'subscriptions', header: 'Assinaturas', render: (row) => row.subscriptionsCount },
  { key: 'payments', header: 'Pagamentos', render: (row) => row.paymentsCount },
  {
    key: 'status',
    header: 'Status',
    render: (row) => <Badge tone={row.isActive ? 'approved' : 'neutral'}>{row.isActive ? 'Ativo' : 'Inativo'}</Badge>,
  },
]

export function CustomersPage() {
  const state = useCustomers()
  const navigate = useNavigate()

  if (state.status === 'loading') return <LoadingState label="Carregando clientes…" />
  if (state.status === 'error') return <ErrorState message={state.error} onRetry={state.refetch} />

  return (
    <Card>
      {state.data.length === 0 ? (
        <EmptyState title="Nenhum cliente cadastrado" description="Ainda não há clientes na plataforma." />
      ) : (
        <DataTable
          columns={columns}
          data={state.data}
          keyExtractor={(row) => row.id}
          onRowClick={(row) => navigate(`/customers/${row.id}`)}
        />
      )}
    </Card>
  )
}
