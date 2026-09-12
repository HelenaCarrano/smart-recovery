import type { SVGProps } from 'react'

/**
 * Ícones de linha minimalistas usados na sidebar e nos indicadores de status.
 * Evita depender de uma biblioteca externa de ícones só para ~8 símbolos.
 */

export function DashboardIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <rect x="3" y="3" width="7" height="9" rx="1.5" />
      <rect x="14" y="3" width="7" height="5" rx="1.5" />
      <rect x="14" y="12" width="7" height="9" rx="1.5" />
      <rect x="3" y="16" width="7" height="5" rx="1.5" />
    </svg>
  )
}

export function PaymentsIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <rect x="2.5" y="5" width="19" height="14" rx="2.25" />
      <path d="M2.5 9.5h19" strokeLinecap="round" />
      <path d="M6 14.5h5" strokeLinecap="round" />
    </svg>
  )
}

export function RecoveryIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <path
        d="M20 11a8 8 0 1 0-2.343 5.657"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path d="M20 5v6h-6" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  )
}

export function CustomersIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <circle cx="9" cy="8" r="3.25" />
      <path d="M2.75 19.25c0-3.176 2.85-5.5 6.25-5.5s6.25 2.324 6.25 5.5" strokeLinecap="round" />
      <path d="M16 8.25a3 3 0 1 1 3.6 4.42" strokeLinecap="round" />
      <path d="M15.5 13.9c2.6.35 4.75 2.16 4.75 4.85" strokeLinecap="round" />
    </svg>
  )
}

export function SubscriptionsIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <rect x="3" y="4.5" width="18" height="16" rx="2" />
      <path d="M3 9.5h18" strokeLinecap="round" />
      <path d="M8 2.5v4M16 2.5v4" strokeLinecap="round" />
      <path d="M7.5 14h3M7.5 17h5" strokeLinecap="round" />
    </svg>
  )
}

export function SettingsIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <circle cx="12" cy="12" r="3" />
      <path
        d="M19.4 13.5a1.7 1.7 0 0 0 .34 1.87l.06.06a2.06 2.06 0 1 1-2.92 2.92l-.06-.06a1.7 1.7 0 0 0-1.87-.34 1.7 1.7 0 0 0-1.03 1.56v.17a2.06 2.06 0 1 1-4.12 0v-.09a1.7 1.7 0 0 0-1.11-1.56 1.7 1.7 0 0 0-1.87.34l-.06.06a2.06 2.06 0 1 1-2.92-2.92l.06-.06a1.7 1.7 0 0 0 .34-1.87 1.7 1.7 0 0 0-1.56-1.03h-.17a2.06 2.06 0 1 1 0-4.12h.09a1.7 1.7 0 0 0 1.56-1.11 1.7 1.7 0 0 0-.34-1.87l-.06-.06a2.06 2.06 0 1 1 2.92-2.92l.06.06a1.7 1.7 0 0 0 1.87.34h.08a1.7 1.7 0 0 0 1.03-1.56v-.17a2.06 2.06 0 1 1 4.12 0v.09a1.7 1.7 0 0 0 1.03 1.56h.08a1.7 1.7 0 0 0 1.87-.34l.06-.06a2.06 2.06 0 1 1 2.92 2.92l-.06.06a1.7 1.7 0 0 0-.34 1.87v.08a1.7 1.7 0 0 0 1.56 1.03h.17a2.06 2.06 0 1 1 0 4.12h-.09a1.7 1.7 0 0 0-1.56 1.03Z"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

export function AlertIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <path
        d="M12 3.5 21.5 20h-19L12 3.5Z"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path d="M12 10v4.5" strokeLinecap="round" />
      <path d="M12 17.25h.01" strokeLinecap="round" />
    </svg>
  )
}

export function CheckCircleIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <circle cx="12" cy="12" r="9" />
      <path d="M8.25 12.25l2.5 2.5 5-5.5" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  )
}

export function MenuIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <path d="M3.5 6.5h17M3.5 12h17M3.5 17.5h17" strokeLinecap="round" />
    </svg>
  )
}

export function CloseIcon(props: SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={1.75} {...props}>
      <path d="M5 5l14 14M19 5 5 19" strokeLinecap="round" />
    </svg>
  )
}
