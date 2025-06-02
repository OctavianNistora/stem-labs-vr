import { Outlet } from "react-router";
import { IconButton } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { createContext, useMemo } from "react";
import {
  closeSnackbar,
  enqueueSnackbar,
  type SnackbarKey,
  SnackbarProvider,
  type VariantType,
} from "notistack";

export type ToastItem = {
  message: string;
  autoHideDuration?: number;
  variant?: VariantType;
};

type ToastState = {
  addToast: (toast: ToastItem) => void;
};

export const ToastContext = createContext<ToastState>({
  addToast: () => null,
});

export default function ToastLayout() {
  function addToast(toast: ToastItem) {
    enqueueSnackbar(toast.message, {
      autoHideDuration: toast.autoHideDuration,
      variant: toast.variant,
    });
  }

  const toastAction = (snackbarId: SnackbarKey) => (
    <IconButton
      size="small"
      aria-label="close"
      color="inherit"
      onClick={() => closeSnackbar(snackbarId)}
    >
      <CloseIcon fontSize="small" />
    </IconButton>
  );

  const toastValue = useMemo(
    () => ({
      addToast,
    }),
    [addToast],
  );

  return (
    <>
      <ToastContext.Provider value={toastValue}>
        <Outlet />
      </ToastContext.Provider>
      <SnackbarProvider
        maxSnack={5}
        autoHideDuration={3000}
        action={toastAction}
      />
    </>
  );
}
