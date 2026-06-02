import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-cursor',
  standalone: true,
  templateUrl: './cursor.html',
  styleUrl: './cursor.scss'
})
export class CursorComponent implements OnInit, OnDestroy {
  private mx = 0;
  private my = 0;
  private rx = 0;
  private ry = 0;
  private animFrame: number = 0;
  private dot!: HTMLElement;
  private ring!: HTMLElement;

  ngOnInit() {
    this.dot  = document.getElementById('cur')!;
    this.ring = document.getElementById('cur-r')!;

    document.addEventListener('mousemove', this.onMouseMove);
    document.addEventListener('mousedown', this.onMouseDown);
    document.addEventListener('mouseup',   this.onMouseUp);
    document.addEventListener('mouseover', this.onHover);

    this.animateRing();
  }

  ngOnDestroy() {
    document.removeEventListener('mousemove', this.onMouseMove);
    document.removeEventListener('mousedown', this.onMouseDown);
    document.removeEventListener('mouseup',   this.onMouseUp);
    document.removeEventListener('mouseover', this.onHover);
    cancelAnimationFrame(this.animFrame);
  }

  // Arrow functions so 'this' is preserved
  private onMouseMove = (e: MouseEvent) => {
    this.mx = e.clientX;
    this.my = e.clientY;
    this.dot.style.left = `${this.mx - 5}px`;
    this.dot.style.top  = `${this.my - 5}px`;
  };

  private onMouseDown = () => {
    this.dot.classList.add('click');
    this.ring.classList.add('click');
  };

  private onMouseUp = () => {
    this.dot.classList.remove('click');
    this.ring.classList.remove('click');
  };

  private onHover = (e: MouseEvent) => {
    const target = e.target as HTMLElement;
    const isClickable = target.closest('a, button, [routerLink], input, select, textarea, .dest-card, .pkg-card, .testi-card');
    if (isClickable) {
      this.dot.classList.add('hover');
      this.ring.classList.add('hover');
    } else {
      this.dot.classList.remove('hover');
      this.ring.classList.remove('hover');
    }
  };

  private animateRing = () => {
    // Smooth lag follow effect
    this.rx += (this.mx - this.rx - 18) * 0.13;
    this.ry += (this.my - this.ry - 18) * 0.13;
    this.ring.style.left = `${this.rx}px`;
    this.ring.style.top  = `${this.ry}px`;
    this.animFrame = requestAnimationFrame(this.animateRing);
  };
}