.error-stack {
  margin-top: 25px;
  background: #f9fafc;
  border: 1px solid #dce2f0;
  border-radius: 8px;
  padding: 15px;
  text-align: left;
  font-size: 0.85rem;
  color: #222;
  overflow-x: auto;
  max-height: 250px;
}

.error-stack h3 {
  margin-top: 0;
  font-size: 0.9rem;
  color: #0038ff;
}

.error-stack pre {
  white-space: pre-wrap;
  margin: 10px 0 0;
  font-family: "Courier New", monospace;
  color: #444;
}
/* ======== Estilos para página de erro customizada PEMAIS ======== */

body.error-page {
  margin: 0;
  padding: 0;
  font-family: 'Inter', 'Segoe UI', Arial, sans-serif;
  background-color: #f4f7fc;
  color: #333;
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
}

.error-container {
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  padding: 40px 50px;
  max-width: 460px;
  width: 90%;
  text-align: center;
  border: 1px solid #e1e6ef;
}

/* Cabeçalho colorido tipo PEMAIS */
.error-container h1 {
  font-size: 2rem;
  font-weight: 800;
  margin-bottom: 20px;
  background: linear-gradient(90deg, #0000ff, #4b0082, #00ff00, #ffff00, #ff0000);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

/* Mensagem principal */
.error-container p {
  margin: 0 0 15px;
  font-size: 1rem;
  color: #444;
  line-height: 1.5;
}

/* Mensagem detalhada do erro */
.error-detail {
  background-color: #f8faff;
  border-left: 4px solid #0038ff;
  padding: 12px 15px;
  border-radius: 6px;
  text-align: left;
  margin-top: 10px;
  color: #0038ff;
  font-size: 0.95rem;
}

/* Botão principal */
.btn {
  display: inline-block;
  padding: 10px 22px;
  background: #0038ff;
  color: #fff;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 600;
  font-size: 1rem;
  margin-top: 25px;
  transition: all 0.2s ease-in-out;
  border: none;
}

.btn:hover {
  background: #0028d4;
  box-shadow: 0 3px 10px rgba(0, 56, 255, 0.3);
  transform: translateY(-1px);
}

/* Rodapé simples */
.error-footer {
  margin-top: 25px;
  font-size: 0.85rem;
  color: #888;
}

/* Responsividade */
@media (max-width: 600px) {
  .error-container {
    padding: 30px 20px;
  }

  .error-container h1 {
    font-size: 1.6rem;
  }

  .btn {
    width: 100%;
  }
}
