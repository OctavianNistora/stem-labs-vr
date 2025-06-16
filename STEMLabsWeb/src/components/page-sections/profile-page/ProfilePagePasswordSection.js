import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useContext, useState } from "react";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import { Button, CircularProgress, Stack, TextField } from "@mui/material";
import { axiosRequestWithAutoReauth } from "../../../helpers/axiosRequestWithAutoReauth.tsx";
export default function ProfilePagePasswordSection() {
    const [newPassword, setNewPassword] = useState("");
    const [newPasswordConfirmation, setNewPasswordConfirmation] = useState("");
    const [currentPassword, setCurrentPassword] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    function handleNewPasswordChange(event) {
        setNewPassword(event.target.value);
    }
    function handleConfirmNewPasswordChange(event) {
        setNewPasswordConfirmation(event.target.value);
    }
    function handleCurrentPasswordChange(event) {
        setCurrentPassword(event.target.value);
    }
    function handleSubmit() {
        const data = {
            newPassword: newPassword,
            currentPassword: currentPassword,
        };
        setIsSubmitting(true);
        axiosRequestWithAutoReauth({
            method: "PUT",
            url: `${import.meta.env.VITE_API_URL}/api/users/${user?.uid}/password`,
            data: data,
            headers: {
                Authorization: `Bearer ${user?.accessToken}`,
            },
        }, setUser)
            .then(() => {
            addToast({
                message: "Password updated successfully",
                variant: "success",
            });
            setNewPassword("");
            setNewPasswordConfirmation("");
            setCurrentPassword("");
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
        })
            .finally(() => {
            setIsSubmitting(false);
        });
    }
    const doPasswordsMatch = !newPassword ||
        !newPasswordConfirmation ||
        newPassword == newPasswordConfirmation;
    return (_jsxs(Stack, { gap: 4, children: [_jsx(TextField, { type: "password", label: "New Password", value: newPassword, onChange: handleNewPasswordChange }), _jsx(TextField, { type: "password", label: "Confirm New Password", error: !doPasswordsMatch, helperText: !doPasswordsMatch ? "Passwords do not match" : undefined, value: newPasswordConfirmation, onChange: handleConfirmNewPasswordChange }), _jsx(TextField, { type: "password", label: "Current Password", value: currentPassword, onChange: handleCurrentPasswordChange }), _jsx(Button, { disabled: !(!!newPassword &&
                    !!newPasswordConfirmation &&
                    !!currentPassword &&
                    doPasswordsMatch) || isSubmitting, variant: "contained", color: "primary", onClick: handleSubmit, children: isSubmitting ? _jsx(CircularProgress, {}) : "Update Password" })] }));
}
