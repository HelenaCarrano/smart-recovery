import { CloseIcon } from '@/components/icons'
import { DISABLED_NAV_ITEM, NAV_ITEMS } from '@/config/navigation'
import { clsx } from 'clsx'
import { NavLink } from 'react-router-dom'

interface SidebarProps {
  /** Controla a exibição em telas pequenas (drawer). Em telas grandes a sidebar é sempre fixa. */
  open: boolean
  onClose: () => void
}

export function Sidebar({ open, onClose }: SidebarProps) {
  return (
    <>
      {/* Overlay do drawer mobile */}
      {open && (
        <div
          className="fixed inset-0 z-30 bg-slate-900/30 lg:hidden"
          onClick={onClose}
          aria-hidden="true"
        />
      )}

      <aside
        className={clsx(
          'fixed inset-y-0 left-0 z-40 flex w-64 shrink-0 flex-col border-r border-slate-200 bg-white transition-transform duration-200 ease-out lg:static lg:translate-x-0',
          open ? 'translate-x-0' : '-translate-x-full',
        )}
      >
        <div className="flex h-16 shrink-0 items-center justify-between px-6">
          <span className="text-lg font-semibold tracking-tight text-slate-900">
            Smart<span className="text-brand-600">Recovery</span>
          </span>
          <button
            type="button"
            onClick={onClose}
            className="text-slate-400 hover:text-slate-600 lg:hidden"
            aria-label="Fechar menu"
          >
            <CloseIcon className="h-5 w-5" />
          </button>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-3 py-4">
          {NAV_ITEMS.map(({ label, path, icon: Icon }) => (
            <NavLink
              key={path}
              to={path}
              end={path === '/'}
              onClick={onClose}
              className={({ isActive }) =>
                clsx(
                  'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                  isActive
                    ? 'bg-brand-50 text-brand-700'
                    : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900',
                )
              }
            >
              <Icon className="h-5 w-5 shrink-0" />
              {label}
            </NavLink>
          ))}

          <div
            className="flex cursor-not-allowed items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-slate-300"
            title="Em breve"
          >
            <DISABLED_NAV_ITEM.icon className="h-5 w-5 shrink-0" />
            {DISABLED_NAV_ITEM.label}
          </div>
        </nav>

        <div className="border-t border-slate-100 px-6 py-4">
          <p className="text-xs text-slate-400">Smart Recovery © 2026</p>
        </div>
      </aside>
    </>
  )
}
