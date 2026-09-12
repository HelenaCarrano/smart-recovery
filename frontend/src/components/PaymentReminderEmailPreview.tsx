import { Card } from '@/components/ui/Card'
import { formatCurrency, formatDateTime } from '@/lib/format'

interface PaymentReminderEmailPreviewProps {
  customerName: string
  amount: number
  executedAt: string | null
}

/**
 * Preview do e-mail de dunning disparado quando o RecoveryDecisionEngine recomenda
 * SendPaymentReminderEmail — não existe envio real de e-mail neste projeto de portfólio,
 * então mostramos o conteúdo que teria sido enviado ao cliente.
 */
export function PaymentReminderEmailPreview({ customerName, amount, executedAt }: PaymentReminderEmailPreviewProps) {
  const firstName = customerName.split(' ')[0]

  return (
    <Card>
      <h2 className="mb-3 text-sm font-semibold text-slate-900">E-mail de regularização enviado</h2>
      <div className="rounded-lg border border-slate-200 bg-slate-50 p-4 text-sm text-slate-700">
        <p className="text-xs text-slate-400">
          Para: {customerName} · {executedAt ? formatDateTime(executedAt) : 'aguardando envio'}
        </p>
        <p className="mt-2 font-semibold text-slate-900">Assunto: Regularize sua assinatura e evite o cancelamento</p>
        <p className="mt-3">Olá, {firstName}!</p>
        <p className="mt-2">
          Não conseguimos confirmar o pagamento da sua assinatura de {formatCurrency(amount)} após algumas tentativas.
          Para continuar aproveitando o serviço sem interrupções, atualize sua forma de pagamento ou regularize via Pix.
        </p>
        <p className="mt-3 inline-block rounded-md bg-brand-600 px-3 py-1.5 text-xs font-medium text-white">
          Atualizar forma de pagamento →
        </p>
      </div>
      <p className="mt-3 text-xs text-slate-400">
        Disparado automaticamente após múltiplas tentativas consecutivas sem sucesso, como alternativa a retentativas
        cegas ou ao cancelamento imediato da assinatura.
      </p>
    </Card>
  )
}
