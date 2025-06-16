import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useContext, useEffect, useRef, useState, } from "react";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import { Button, CircularProgress, Stack } from "@mui/material";
import { ProfileStyledTextField } from "../../ProfileStyledMUIComponents.tsx";
import { isMobilePhone } from "validator";
import { axiosRequestWithAutoReauth } from "../../../helpers/axiosRequestWithAutoReauth.tsx";
export default function ProfilePageProfileSection() {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [isEditMode, setIsEditMode] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    const lastKnownProfileData = useRef(null);
    useEffect(() => {
        if (!user) {
            return;
        }
        axiosRequestWithAutoReauth({
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/profile`,
            headers: {
                Authorization: `Bearer ${user.accessToken}`,
            },
        }, setUser)
            .then((response) => {
            const { firstName, lastName, phoneNumber } = response.data;
            setFirstName(firstName);
            setLastName(lastName);
            setPhoneNumber(phoneNumber);
            lastKnownProfileData.current = {
                firstName,
                lastName,
                phoneNumber,
            };
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
        });
    }, []);
    function handleFirstNameChange(event) {
        setFirstName(event.target.value);
    }
    function handleLastNameChange(event) {
        setLastName(event.target.value);
    }
    function handlePhoneNumberChange(event) {
        setPhoneNumber(event.target.value);
    }
    function handleEditClick() {
        setIsEditMode(true);
    }
    function handleSaveClick() {
        if (isEditMode) {
            const data = {
                firstName,
                lastName,
                phoneNumber,
            };
            setIsSubmitting(true);
            axiosRequestWithAutoReauth({
                method: "PUT",
                url: `${import.meta.env.VITE_API_URL}/api/users/${user?.uid}/profile`,
                data: data,
                headers: {
                    Authorization: `Bearer ${user?.accessToken}`,
                },
            }, setUser)
                .then(() => { })
                .catch((error) => {
                setFirstName(lastKnownProfileData.current?.firstName || "");
                setLastName(lastKnownProfileData.current?.lastName || "");
                setPhoneNumber(lastKnownProfileData.current?.phoneNumber || "");
                toastErrorMessageHandle(addToast, setUser, error);
            })
                .finally(() => {
                setIsSubmitting(false);
                setIsEditMode(false);
            });
        }
    }
    return (_jsxs(Stack, { gap: 4, children: [_jsx(ProfileStyledTextField, { readOnly: !isEditMode, label: "First Name", placeholder: isEditMode ? undefined : "Empty", value: firstName, onChange: handleFirstNameChange }), _jsx(ProfileStyledTextField, { readOnly: !isEditMode, label: "Last Name", placeholder: isEditMode ? undefined : "Empty", value: lastName, onChange: handleLastNameChange }), _jsx(ProfileStyledTextField, { readOnly: !isEditMode, label: "Phone Number", placeholder: isEditMode ? undefined : "Empty", error: !!phoneNumber && !isMobilePhone(phoneNumber), helperText: !!phoneNumber && !isMobilePhone(phoneNumber)
                    ? "Invalid phone number"
                    : undefined, value: phoneNumber, onChange: handlePhoneNumberChange }), isEditMode ? (_jsx(Button, { disabled: isSubmitting, variant: "contained", color: "primary", onClick: handleSaveClick, children: isSubmitting ? _jsx(CircularProgress, {}) : "Save Changes" })) : (_jsx(Button, { variant: "outlined", color: "primary", onClick: handleEditClick, children: "Edit Profile" }))] }));
}
