import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from './shared/navbar/navbar';
import { FooterComponent } from './shared/footer/footer';
import { CursorComponent } from './components/cursor/cursor';
import { ToastComponent } from './shared/toast/toast';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, FooterComponent,Navbar, CursorComponent, ToastComponent],
  templateUrl: './app.html'
})
export class App {}