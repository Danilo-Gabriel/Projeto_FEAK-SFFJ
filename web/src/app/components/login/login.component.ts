import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from '../../../environment/environment';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit{

  public formLogin!: FormGroup;

  constructor(public formBuilder: FormBuilder,
    private router : Router,
    private http : HttpClient,
  ){}


  ngOnInit(): void {
    this.Formulario();
  }

  Formulario(){
    this.formLogin = this.formBuilder.group({
      login: ['', Validators.required],
      senha: ['', Validators.required]
    })
  }

  onSubmit() {
    console.log(this.formLogin, "TESTE");
      this.router.navigate([ '/pages']);
    console.log(environment.endPoint);
  }


  //CRIAR SERVICES E REFATORAR CODIGOS 

  


}
