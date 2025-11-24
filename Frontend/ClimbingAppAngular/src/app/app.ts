import { Component, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ClimbCard } from './climb-card/climb-card';
import { ClimbList } from './climb-list/climb-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, ClimbCard, ClimbList],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ClimbingAppAngular');
}
