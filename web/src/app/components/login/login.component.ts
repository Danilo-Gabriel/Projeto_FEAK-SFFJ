import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from '../../../environment/environment';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { LoginDTO } from '../../models/dto/login-dto';
import { ServiceResponse } from '../../models/dto/service-response';
import { catchError, Observable, throwError } from 'rxjs';
import { AppMessageService } from '../../services/app-message.service';

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
    private message : AppMessageService
  ){}


  ngOnInit(): void {
    this.Formulario();
  }

  Formulario(){
    this.formLogin = this.formBuilder.group({
      nomeLogin: ['', Validators.required],
      senha: ['', Validators.required]
    })
  }

  onSubmit() {

    console.log(this.formLogin)
    if(this.formLogin.valid){
      const loginDTO: LoginDTO = this.formLogin.value as LoginDTO;
      this.login(loginDTO);
    }
  
  }


  login(record: LoginDTO): void {
    this.http.post<ServiceResponse<LoginDTO>>(`${environment.endPoint}/login`, record, { responseType: 'json' }).pipe(
    catchError((error) => {
      return throwError(() => error);
    })
  ).subscribe({
        next: (res) => {
          console.log('Login OK:', res);
          this.message.showSuccess("")
          this.router.navigate(['pages'])
        },
        error: (err) => {
          console.error('Erro ao logar:', err);
          this.message.showError("Usuário ou senha incorretas")
        }
      });
}

  //CRIAR SERVICES E REFATORAR CODIGOS 

  


}
