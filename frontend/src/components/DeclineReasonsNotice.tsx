import { Card } from '@/components/ui/Card'

/**
 * A API não tem hoje um endpoint que liste todos os pagamentos ou agregue motivos
 * de recusa — só GetById e GetByCustomer (ver PaymentsController). Sem isso não dá
 * para montar essa distribuição com dados reais, então mostramos essa limitação
 * de forma explícita em vez de inventar números.
 */
export function DeclineReasonsNotice() {
  return (
    <Card>
      <h2 className="mb-2 text-sm font-semibold text-slate-900">Motivos de recusa</h2>
      <p className="text-sm text-slate-500">
        A API ainda não expõe uma distribuição agregada dos motivos de recusa — isso exigiria um endpoint de
        listagem de pagamentos ou um agregado dedicado, que não existe hoje. Assim que esse dado estiver
        disponível no backend, este bloco passa a exibir o gráfico real.
      </p>
    </Card>
  )
}
