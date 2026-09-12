import { AppLayout } from '@/layouts/AppLayout'
import { CustomersPage } from '@/pages/CustomersPage'
import { DashboardPage } from '@/pages/DashboardPage'
import { PaymentDetailPage } from '@/pages/PaymentDetailPage'
import { PaymentsPage } from '@/pages/PaymentsPage'
import { RecoveryPage } from '@/pages/RecoveryPage'
import { SubscriptionsPage } from '@/pages/SubscriptionsPage'
import { Route, Routes } from 'react-router-dom'

function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/payments" element={<PaymentsPage />} />
        <Route path="/payments/:id" element={<PaymentDetailPage />} />
        <Route path="/recovery" element={<RecoveryPage />} />
        <Route path="/customers" element={<CustomersPage />} />
        <Route path="/subscriptions" element={<SubscriptionsPage />} />
      </Route>
    </Routes>
  )
}

export default App
