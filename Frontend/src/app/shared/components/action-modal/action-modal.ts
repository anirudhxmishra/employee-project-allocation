
import { Component, EventEmitter, Input, Output } from '@angular/core';

export type ModalState = 'confirm' | 'success' | 'error';

@Component({
  selector: 'app-action-modal',
  standalone: true,
  templateUrl: './action-modal.html',
  styleUrl: './action-modal.scss'
})
export class ActionModal {

  @Input() visible = false;

  @Input() state: ModalState = 'confirm';

  @Input() title = '';

  @Input() message = '';

  @Input() employeeName = '';

  @Input() employeeCode = '';

  @Input() confirmText = 'Yes, Continue';

  @Input() cancelText = 'Cancel';

  @Input() isProcessing = false;

  @Output() confirmed = new EventEmitter<void>();

  @Output() cancelled = new EventEmitter<void>();

  onConfirm(): void {
    if (!this.isProcessing) {
      this.confirmed.emit();
    }
  }

  onCancel(): void {
    if (!this.isProcessing) {
      this.cancelled.emit();
    }
  }

  onBackdropClick(event: MouseEvent): void {
    if (event.target === event.currentTarget && !this.isProcessing) {
      this.cancelled.emit();
    }
  }
}
