import { ApiStatusIndicator } from '@/components/ApiStatusIndicator'
import { MenuIcon } from '@/components/icons'
import { NAV_ITEMS } from '@/config/navigation'
import { useLocation } from 'react-router-dom'

interface HeaderProps {
  onOpenMenu: () => void
}

/** Encontra o item de navegação cujo path mais se aproxima da rota atual (para páginas de detalhe aninhadas). */
function getPageMeta(pathname: string) {
  const exactMatch = NAV_ITEMS.find((item) => item.path === pathname)
  if (exactMatch) return exactMatch

  const prefixMatch = NAV_ITEMS.filter((item) => item.path !== '/' && pathname.startsWith(item.path)).sort(
    (a, b) => b.path.length - a.path.length,
  )[0]

  return prefixMatch ?? NAV_ITEMS[0]
}

export function Header({ onOpenMenu }: HeaderProps) {
  const { pathname } = useLocation()
  const { label, description } = getPageMeta(pathname)

  return (
    <header className="sticky top-0 z-20 flex h-16 shrink-0 items-center gap-4 border-b border-slate-200 bg-white/80 px-4 backdrop-blur sm:px-6">
      <button
        type="button"
        onClick={onOpenMenu}
        className="text-slate-400 hover:text-slate-600 lg:hidden"
        aria-label="Abrir menu"
      >
        <MenuIcon className="h-5 w-5" />
      </button>

      <div className="min-w-0 flex-1">
        <h1 className="truncate text-lg font-semibold text-slate-900">{label}</h1>
        <p className="truncate text-sm text-slate-500">{description}</p>
      </div>

      <ApiStatusIndicator />
    </header>
  )
}
