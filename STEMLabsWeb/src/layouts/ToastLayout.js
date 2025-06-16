import { jsx as _jsx, Fragment as _Fragment, jsxs as _jsxs } from "react/jsx-runtime";
import { Outlet } from "react-router";
import { IconButton } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { createContext, useMemo } from "react";
import { closeSnackbar, enqueueSnackbar, SnackbarProvider, } from "notistack";
export const ToastContext = createContext({
    addToast: () => null,
});
export default function ToastLayout() {
    function addToast(toast) {
        enqueueSnackbar(toast.message, {
            autoHideDuration: toast.autoHideDuration,
            variant: toast.variant,
        });
    }
    const toastAction = (snackbarId) => (_jsx(IconButton, { size: "small", "aria-label": "close", color: "inherit", onClick: () => closeSnackbar(snackbarId), children: _jsx(CloseIcon, { fontSize: "small" }) }));
    const toastValue = useMemo(() => ({
        addToast,
    }), [addToast]);
    return (_jsxs(_Fragment, { children: [_jsx(ToastContext.Provider, { value: toastValue, children: _jsx(Outlet, {}) }), _jsx(SnackbarProvider, { maxSnack: 5, autoHideDuration: 3000, action: toastAction })] }));
}
