import {
  CustomersIcon,
  DashboardIcon,
  PaymentsIcon,
  RecoveryIcon,
  SettingsIcon,
  SubscriptionsIcon,
} from '@/components/icons'
import type { ComponentType, SVGProps } from 'react'

export interface NavItem {
  label: string
  path: string
  description: string
  icon: ComponentType<SVGProps<SVGSVGElement>>
}

/**
 * Fonte única de verdade para a navegação: a Sidebar lista estes itens e o
 * Header deriva o título/descrição da página a partir da rota atual.
 */
export const NAV_ITEMS: NavItem[] = [
  {
    label: 'Dashboard',
    path: '/',
    description: 'Receita recuperada, recusas e oportunidades de recuperação em um só lugar.',
    icon: DashboardIcon,
  },
  {
    label: 'Pagamentos',
    path: '/payments',
    description: 'Histórico completo de cobranças recorrentes e seus status.',
    icon: PaymentsIcon,
  },
  {
    label: 'Recuperação',
    path: '/recovery',
    description: 'Oportunidades de recuperação de receita ordenadas por impacto.',
    icon: RecoveryIcon,
  },
  {
    label: 'Clientes',
    path: '/customers',
    description: 'Assinantes da plataforma e seu histórico de cobrança.',
    icon: CustomersIcon,
  },
  {
    label: 'Assinaturas',
    path: '/subscriptions',
    description: 'Planos ativos, periodicidade e próximas cobranças.',
    icon: SubscriptionsIcon,
  },
]

/** Item visual desabilitado — ainda não é uma rota navegável. */
export const DISABLED_NAV_ITEM = {
  label: 'Configurações',
  icon: SettingsIcon,
}
