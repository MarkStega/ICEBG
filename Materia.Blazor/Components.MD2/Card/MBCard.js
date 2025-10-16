import { MDCRipple } from '@material/ripple';
export function init(elem) {
    if (!elem) {
        return;
    }
    elem._ripple = MDCRipple.attachTo(elem);
}
