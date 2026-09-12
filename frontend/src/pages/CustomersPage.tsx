import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { type DataTableColumn, DataTable } from '@/components/ui/DataTable'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import { Pagination } from '@/components/ui/Pagination'
import { useCustomers } from '@/hooks/useCustomers'
import type { CustomerListItem } from '@/types/customerListItem'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

const PAGE_SIZE = 20

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
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const state = useCustomers(page, PAGE_SIZE, search)
  const navigate = useNavigate()

  return (
    <div className="space-y-6">
      <Card>
        <input
          type="search"
          placeholder="Buscar por nome ou email…"
          value={search}
          onChange={(e) => {
            setSearch(e.target.value)
            setPage(1)
          }}
          className="w-full max-w-sm rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none focus:ring-1 focus:ring-brand-400"
        />
      </Card>

      {state.status === 'loading' && <LoadingState label="Carregando clientes…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' &&
        (state.data.items.length === 0 ? (
          <Card>
            <EmptyState
              title="Nenhum cliente encontrado"
              description={search ? 'Nenhum cliente corresponde à busca.' : 'Ainda não há clientes na plataforma.'}
            />
          </Card>
        ) : (
          <Card>
            <DataTable
              columns={columns}
              data={state.data.items}
              keyExtractor={(row) => row.id}
              onRowClick={(row) => navigate(`/customers/${row.id}`)}
            />
            <Pagination
              page={state.data.page}
              pageSize={state.data.pageSize}
              totalCount={state.data.totalCount}
              onPageChange={setPage}
            />
          </Card>
        ))}
    </div>
  )
}
