import { Route, Routes } from 'react-router-dom'

/**
 * Placeholder temporário da Etapa 1 (setup do projeto). Será substituído pelo
 * layout real (sidebar + header + rotas de página) na Etapa 2.
 */
function SetupPlaceholder() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-50">
      <div className="rounded-xl border border-slate-200 bg-white px-10 py-8 text-center shadow-sm">
        <p className="text-sm font-medium text-brand-600">Smart Recovery</p>
        <h1 className="mt-1 text-2xl font-semibold text-slate-900">
          Frontend configurado
        </h1>
        <p className="mt-2 text-sm text-slate-500">
          React + TypeScript + Vite + Tailwind prontos. Layout na próxima etapa.
        </p>
      </div>
    </div>
  )
}

function App() {
  return (
    <Routes>
      <Route path="/" element={<SetupPlaceholder />} />
    </Routes>
  )
}

export default App
