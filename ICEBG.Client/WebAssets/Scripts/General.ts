import { MDCRipple } from '@material/ripple';
import { MDCDialog } from '@material/dialog';

export function instantiateErrorDialog(): void {
    new MDCRipple(document.getElementById('reload-button')!);

    var dialog = document.getElementById('reload-dialog')!;
    var container = document.getElementById('reload-container')!;
    var scrim = document.getElementById('reload-scrim')!;

    var mdcDialog = new MDCDialog(dialog);
    mdcDialog.escapeKeyAction = '';
    mdcDialog.scrimClickAction = '';

    dialog.style.display = 'flex';
    container.style.opacity = '1';
    scrim.style.opacity = '1';
}

export function scrollToTop() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
}
