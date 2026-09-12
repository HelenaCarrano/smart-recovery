import { Card } from '@/components/ui/Card'

/**
 * Explica o propósito do produto para quem abre o Dashboard pela primeira vez — recrutadores e
 * avaliadores não necessariamente conhecem o domínio de recuperação de pagamentos recorrentes.
 * Fica só no Dashboard (a "porta de entrada" do app) para não repetir a mesma explicação em toda página.
 */
export function DashboardIntro() {
  return (
    <Card className="border-brand-100 bg-brand-50/40">
      <p className="text-sm text-slate-700">
        Quando uma cobrança recorrente é recusada, o Smart Recovery calcula um{' '}
        <span className="font-semibold text-brand-700">Recovery Score</span> (0–100) a partir do histórico do
        cliente e do motivo da recusa, e recomenda automaticamente a melhor estratégia — retry, pedir
        atualização de pagamento, revisão manual ou cancelamento. Os números abaixo mostram esse processo
        rodando sobre dados simulados.
      </p>
    </Card>
  )
}
