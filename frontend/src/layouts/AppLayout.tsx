import { Header } from '@/components/Header'
import { Sidebar } from '@/components/Sidebar'
import { useState } from 'react'
import { Outlet, useLocation } from 'react-router-dom'

export function AppLayout() {
  const [mobileNavOpen, setMobileNavOpen] = useState(false)
  const { pathname } = useLocation()

  return (
    <div className="flex min-h-screen bg-slate-50">
      <Sidebar open={mobileNavOpen} onClose={() => setMobileNavOpen(false)} />

      <div className="flex min-w-0 flex-1 flex-col">
        <Header onOpenMenu={() => setMobileNavOpen(true)} />

        <main className="flex-1 px-4 py-6 sm:px-6 lg:px-8">
          {/* key={pathname} força o React a remontar o conteúdo a cada rota, reiniciando a animação de entrada. */}
          <div key={pathname} className="animate-page-in">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  )
}
