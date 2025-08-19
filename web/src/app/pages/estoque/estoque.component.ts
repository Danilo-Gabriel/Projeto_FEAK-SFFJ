import { Component, NgModule, OnInit } from '@angular/core';

// A interface Product ajuda a manter a estrutura dos objetos de produto.
export interface Product {
  id: number;
  name: string;
  quantity: number;
  price: number;
}

@Component({
  selector: 'app-estoque',
  templateUrl: './estoque.component.html',
  styleUrls: ['./estoque.component.scss']
})
export class EstoqueComponent implements OnInit {

  products: Product[] = [
    { id: 1, name: 'Notebook Dell', quantity: 15, price: 5500.00 },
    { id: 2, name: 'Mouse Gamer', quantity: 50, price: 150.00 },
    { id: 3, name: 'Teclado Mecânico', quantity: 30, price: 300.00 },
  ];

  newProduct: Product = { id: 0, name: '', quantity: 0, price: 0 };

  // Variável para controle da edição.
  isEditing = false;
  editingProduct: Product | null = null;

  constructor() { }

  ngOnInit(): void {
  }

  // Função para adicionar um novo produto.
  addProduct(): void {
    if (this.newProduct.name && this.newProduct.quantity > 0 && this.newProduct.price > 0) {
      // Atribui um ID ao novo produto (simulando um banco de dados).
      this.newProduct.id = this.products.length > 0 ? Math.max(...this.products.map(p => p.id)) + 1 : 1;
      this.products.push({ ...this.newProduct }); // Adiciona uma cópia do objeto.
      this.clearNewProductForm();
    } else {
      alert('Por favor, preencha todos os campos corretamente.');
    }
  }

  // Prepara o formulário para editar um produto existente.
  editProduct(product: Product): void {
    this.isEditing = true;
    this.editingProduct = { ...product }; // Copia o produto para não modificar o original diretamente.
  }

  // Salva as alterações de um produto.
  saveEdit(): void {
    if (this.editingProduct) {
      const index = this.products.findIndex(p => p.id === this.editingProduct?.id);
      if (index > -1) {
        this.products[index] = { ...this.editingProduct };
        this.cancelEdit(); // Sai do modo de edição.
      }
    }
  }

  // Cancela a edição e limpa o formulário.
  cancelEdit(): void {
    this.isEditing = false;
    this.editingProduct = null;
  }

  // Remove um produto da lista.
  deleteProduct(id: number): void {
    if (confirm('Tem certeza que deseja excluir este produto?')) {
      this.products = this.products.filter(p => p.id !== id);
    }
  }

  // Limpa os campos do formulário de novo produto.
  private clearNewProductForm(): void {
    this.newProduct = { id: 0, name: '', quantity: 0, price: 0 };
  }
}
