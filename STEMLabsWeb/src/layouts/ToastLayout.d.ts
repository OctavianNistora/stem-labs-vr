import { type VariantType } from "notistack";
export type ToastItem = {
    message: string;
    autoHideDuration?: number;
    variant?: VariantType;
};
type ToastState = {
    addToast: (toast: ToastItem) => void;
};
export declare const ToastContext: import("react").Context<ToastState>;
export default function ToastLayout(): import("react/jsx-runtime").JSX.Element;
export {};
