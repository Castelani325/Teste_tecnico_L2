import { useEffect, useState } from 'react';
import { FiltrosPagamentos } from './components/FiltrosPagamentos';
import { PagamentosTable } from './components/PagamentosTable';
import { listarPagamentos } from './services/api';
import type { FiltrosPagamento, Pagamento } from './types/pagamento';
import './App.css';

function App() {
  const [pagamentos, setPagamentos] = useState<Pagamento[]>([]);
  const [filtros, setFiltros] = useState<FiltrosPagamento>({});
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      const dados = await listarPagamentos(filtros);
      setPagamentos(dados);
    } catch {
      setErro('Não foi possível carregar os pagamentos. Verifique se a API está rodando.');
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtros]);

  const totalErros = pagamentos.filter((p) => p.status.trim().toLowerCase() === 'erro').length;

  return (
    <div className="app">
      <header>
        <h1>Sabemi — Painel de Pagamentos</h1>
      </header>

      <FiltrosPagamentos filtros={filtros} onChange={setFiltros} onAtualizar={carregar} />

      {erro && <p className="mensagem-erro">{erro}</p>}

      {!erro && totalErros > 0 && (
        <p className="alerta-eventos-erro">
          ⚠️ {totalErros} pagamento{totalErros > 1 ? 's' : ''} com status de erro encontrado{totalErros > 1 ? 's' : ''}.
        </p>
      )}

      <PagamentosTable pagamentos={pagamentos} carregando={carregando} />
    </div>
  );
}

export default App;
