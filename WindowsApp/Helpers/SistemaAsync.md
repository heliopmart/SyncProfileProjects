# Design do Sistema de Sincronização Local ↔ Nuvem

Este documento detalha a arquitetura, estratégias e lógica para implementar o sistema de sincronização de arquivos locais com a nuvem, conforme discutido.

---

## **Opção Alternativa: Sincronização Imediata + Verificação Parcial**

### **Objetivo**
Garantir sincronização eficiente entre arquivos locais e na nuvem, equilibrando:
- **Consumo da API**.
- **Desempenho local e de rede**.
- **Experiência do usuário (UX)**.

### **Componentes do Sistema**

1. **Sincronização Imediata**:
   - Ações prioritárias (criação, modificação) disparam o upload imediato.
   - Temporizador de debounce para arquivos em edição contínua.

2. **Verificação Parcial**:
   - Sincronizações periódicas corrigem possíveis falhas e garantem consistência.

3. **Fila de Processamento (Queue)**:
   - Controle de arquivos modificados.
   - Ordenação e agrupamento por prioridade e projeto/pasta.

4. **Batch Processing**:
   - Agrupamento de múltiplos arquivos para reduzir requisições.

5. **Processamento Paralelo com Limites**:
   - Upload de arquivos em paralelo com um número limitado de conexões simultâneas.

6. **Verificação de Conteúdo**:
   - Comparação de hashes/timestamps para evitar uploads redundantes.

---

## **Lógica do Sistema**

### **1. Sincronização Imediata (Debounce)**
- **Detectar alterações**:
  - O sistema monitora mudanças locais (e.g., criação, modificação de arquivos).
  - Para cada alteração detectada, o arquivo é adicionado à fila com um timestamp.

- **Temporizador de debounce**:
  - Se novas alterações ocorrerem antes do temporizador expirar, o temporizador é redefinido.
  - Apenas a versão final do arquivo é enviada.

- **Upload diferido**:
  - Após o tempo de debounce, o arquivo é enviado para a nuvem.

**Exemplo:**
1. `Receitas.xlsx` é modificado continuamente entre 14:00:00 e 14:00:10.
2. O temporizador de debounce aguarda até que as alterações cessem.
3. Às 14:00:10, o upload da versão mais recente é realizado.

---

### **2. Fila de Processamento**
- Cada alteração é registrada na fila com:
  - Identificador do arquivo.
  - Timestamp da alteração.
  - Tipo de alteração (criação, modificação, exclusão, renomeação).

- A fila processa os arquivos de forma ordenada:
  - Prioridade para alterações mais recentes.
  - Agrupamento por pasta ou projeto.

---

### **3. Agrupamento e Batch Processing**
- Alterações em múltiplos arquivos são agrupadas por pasta ou projeto.
- Um único upload é realizado para sincronizar todos os arquivos do grupo.

**Exemplo:**
1. Entre 14:00:00 e 14:00:10, 5 arquivos são modificados na pasta `Projeto A`.
2. O sistema agrupa essas alterações.
3. Um único request para a API da Box.com é feito para sincronizar os 5 arquivos.

---

### **4. Verificação Periódica**
- Em intervalos regulares, o sistema verifica:
  - Arquivos locais e na nuvem.
  - Diferenças de conteúdo (hash/timestamp).
  - Consistência entre metadados no Firestore e na nuvem.

- Corrige problemas como:
  - Uploads falhos.
  - Arquivos criados offline e não sincronizados.

**Exemplo:**
1. Às 15:00:00, o sistema realiza uma verificação.
2. Detecta que `Notas.txt`, criado offline, ainda não foi enviado.
3. O arquivo é sincronizado com a nuvem.

---

### **5. Processamento Paralelo com Limites**
- Para otimizar uploads:
  - O sistema permite múltiplos uploads simultâneos.
  - Define um limite (e.g., 5 conexões paralelas).

**Exemplo:**
1. 50 arquivos foram alterados.
2. O sistema inicia 5 uploads simultâneos.
3. Assim que um upload é concluído, outro da fila é iniciado.

---

## **Tratamento de Cenários Específicos**

### **Arquivos em Edição Contínua**
- Detecção via debounce para evitar uploads redundantes.
- Upload apenas após intervalo de inatividade.

### **Alterações Simultâneas em Múltiplos Arquivos**
- Agrupamento por pasta/projeto.
- Batch processing para reduzir requisições.
- Upload paralelo com limite configurado.

### **Renomeações e Movimentações**
- Atualização de metadados no Firestore sem necessidade de reupload.

### **Exclusões**
- Solicitar confirmação do usuário para exclusões detectadas na nuvem ou local.

---

## **Benefícios da Opção Alternativa**

1. **Redução de Consumo da API**:
   - Uploads em lote e sincronizações periódicas economizam recursos.

2. **Melhoria de Desempenho**:
   - Evita sobrecarga do dispositivo e da rede.

3. **Experiência do Usuário (UX)**:
   - Sincronização fluida e confiável.

4. **Consistência Garantida**:
   - Verificações periódicas corrigem falhas.

---

## **Estrutura Recomendável para Implementação**

### **Classes Principais no C#**
1. `FileWatcher`:
   - Monitora alterações locais.
   - Gatilho para adicionar arquivos à fila.

2. `SyncQueue`:
   - Gerencia a fila de arquivos modificados.
   - Controla a ordem e o agrupamento.

3. `BatchUploader`:
   - Processa uploads em lote.
   - Gerencia paralelismo e limites.

4. `SyncProcessor`:
   - Realiza sincronizações periódicas.
   - Verifica consistência entre local, Firestore e nuvem.

5. `FirestoreManager`:
   - Gerencia metadados no Firestore.
   - Atualiza histórico e status dos arquivos.

6. `BoxAuthenticator`:
   - Interface para interagir com a API da Box.com.

---

## **Conclusão**
Esta abordagem combina eficiência, flexibilidade e confiabilidade para sincronização de arquivos locais com a nuvem. As estratégias discutidas otimizam o consumo de recursos e garantem uma experiência fluida para o usuário final.

Próximos passos incluem:
- Implementação do sistema de fila e debounce.
- Integração com a API da Box.com.
- Testes para cenários de carga e alterações simultâneas.
