import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Customer } from '../../models/dto/customer';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-usuario',
  templateUrl: './usuario.component.html',
  styleUrl: './usuario.component.scss'
})
export class UsuarioComponent implements OnInit {

    @ViewChild('dt1') dt1! : Table
    customers!: Customer[];
    selectedCustomers!: Customer;

  ngOnInit(): void {
    this.customers = [{
                id: 1000,
                name: 'James Butt',
                country: {
                    name: 'Algeria',
                    code: 'dz'
                },
                company: 'Benton, John B Jr',
                date: '2015-09-13',
                status: 'unqualified',
                verified: true,
                activity: 17,
                representative: {
                    name: 'Ioni Bowcher',
                    image: 'ionibowcher.png'
                },
                balance: 70663
            },
            {
                id: 1001,
                name: 'Josephine Darakjy',
                country: {
                    name: 'Egypt',
                    code: 'eg'
                },
                company: 'Chanay, Jeffrey A Esq',
                date: '2019-02-09',
                status: 'proposal',
                verified: true,
                activity: 0,
                representative: {
                    name: 'Amy Elsner',
                    image: 'amyelsner.png'
                },
                balance: 82429
            },];
  }

  getSeverity(status: string): 'success' | 'secondary' | 'info' | 'warning' | 'danger' | 'contrast' | undefined {
  switch (status) {
    case 'unqualified':
      return 'danger';
    case 'qualified':
      return 'success';
    case 'new':
      return 'info';
    case 'negotiation':
      return 'warning';
    case 'renewal':
      return 'secondary'; // Substitua 'null' por algo válido ou 'undefined'
    default:
      return undefined;
  }
}

onFilterGlobal(event: Event) {
  const input = event.target as HTMLInputElement;
  const value = input?.value ?? '';
  this.dt1.filterGlobal(value, 'contains');
}

}
