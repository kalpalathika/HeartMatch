import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from './nav/nav.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  title = 'client';
  http = inject(HttpClient) // injected service here using HttpClient
  users: any;

  ngOnInit(): void {
      this.http.get('http://localhost:5000/api/users').subscribe({
        next: (response: any)  => this.users = response,
        error: (error: any) => console.log(error),
        complete: () => console.log('Request has completed.')
      })
  }
}
