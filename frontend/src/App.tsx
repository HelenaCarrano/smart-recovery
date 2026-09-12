import { AppLayout } from '@/layouts/AppLayout'
import { LoadingState } from '@/components/ui/LoadingState'
import { lazy, Suspense } from 'react'
import { Route, Routes } from 'react-router-dom'

// Uma página por chunk: nenhuma delas é pequena (tabelas, gráficos, formatação), e o usuário
// só paga pelo código da página que está de fato visitando.
const DashboardPage = lazy(() => import('@/pages/DashboardPage').then((m) => ({ default: m.DashboardPage })))
const PaymentsPage = lazy(() => import('@/pages/PaymentsPage').then((m) => ({ default: m.PaymentsPage })))
const PaymentDetailPage = lazy(() =>
  import('@/pages/PaymentDetailPage').then((m) => ({ default: m.PaymentDetailPage })),
)
const RecoveryPage = lazy(() => import('@/pages/RecoveryPage').then((m) => ({ default: m.RecoveryPage })))
const CustomersPage = lazy(() => import('@/pages/CustomersPage').then((m) => ({ default: m.CustomersPage })))
const CustomerDetailPage = lazy(() =>
  import('@/pages/CustomerDetailPage').then((m) => ({ default: m.CustomerDetailPage })),
)
const SubscriptionsPage = lazy(() =>
  import('@/pages/SubscriptionsPage').then((m) => ({ default: m.SubscriptionsPage })),
)

function App() {
  return (
    <Suspense fallback={<LoadingState label="Carregando página…" />}>
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="/payments" element={<PaymentsPage />} />
          <Route path="/payments/:id" element={<PaymentDetailPage />} />
          <Route path="/recovery" element={<RecoveryPage />} />
          <Route path="/customers" element={<CustomersPage />} />
          <Route path="/customers/:id" element={<CustomerDetailPage />} />
          <Route path="/subscriptions" element={<SubscriptionsPage />} />
        </Route>
      </Routes>
    </Suspense>
  )
}

export default App
