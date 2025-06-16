import { jsx as _jsx, Fragment as _Fragment, jsxs as _jsxs } from "react/jsx-runtime";
import { useNavigate, useParams } from "react-router";
import { Box, Button, CircularProgress, IconButton, Stack, TextField, } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import RemoveIcon from "@mui/icons-material/Remove";
import AddIcon from "@mui/icons-material/Add";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function ManageLaboratoryPage() {
    const [name, setName] = useState("");
    const [sceneId, setSceneId] = useState("");
    const [checkListStepList, setCheckListStepList] = useState([""]);
    const [isReceivingData, setIsReceivingData] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { laboratoryId } = useParams();
    const navigate = useNavigate();
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    useEffect(() => {
        if (!isStringPositiveInteger(laboratoryId) && laboratoryId != "-1") {
            navigate("..");
            return;
        }
        if (laboratoryId == "-1") {
            setIsReceivingData(false);
            return;
        }
        axiosRequestWithAutoReauth({
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/laboratories/${laboratoryId}`,
            headers: {
                Authorization: `Bearer ${user?.accessToken}`,
            },
        }, setUser)
            .then((response) => {
            const lab = response.data;
            setName(lab.name);
            setSceneId(lab.sceneId.toString());
            setCheckListStepList(lab.steps);
            setIsReceivingData(false);
        })
            .catch((error) => {
            navigate("..");
            toastErrorMessageHandle(addToast, setUser, error);
        });
    }, []);
    function handleNameChange(event) {
        setName(event.target.value);
    }
    function handleSceneIdChange(event) {
        setSceneId(event.target.value);
    }
    function handleCheckListStepList(event, index) {
        setCheckListStepList((prevSteps) => {
            const newSteps = [...prevSteps];
            newSteps[index] = event.target.value;
            return newSteps;
        });
    }
    function handleSubmit() {
        const data = {
            name: name,
            sceneId: parseInt(sceneId),
            steps: checkListStepList,
        };
        setIsSubmitting(true);
        if (laboratoryId == "-1") {
            axiosRequestWithAutoReauth({
                method: "POST",
                url: `${import.meta.env.VITE_API_URL}/api/laboratories`,
                data: data,
                headers: {
                    Authorization: `Bearer ${user?.accessToken}`,
                },
            }, setUser)
                .then(() => {
                navigate("..");
            })
                .catch((error) => {
                toastErrorMessageHandle(addToast, setUser, error);
            })
                .finally(() => {
                setIsSubmitting(false);
            });
        }
        else {
            axiosRequestWithAutoReauth({
                method: "PUT",
                url: `${import.meta.env.VITE_API_URL}/api/laboratories/${laboratoryId}`,
                data: data,
                headers: {
                    Authorization: `Bearer ${user?.accessToken}`,
                },
            }, setUser)
                .then(() => {
                navigate("..");
            })
                .catch((error) => {
                toastErrorMessageHandle(addToast, setUser, error);
            })
                .finally(() => {
                setIsSubmitting(false);
            });
        }
    }
    function addCheckListStep() {
        setCheckListStepList((prevSteps) => [...prevSteps, ""]);
    }
    function removeLastCheckListStep() {
        setCheckListStepList((prevSteps) => prevSteps.slice(0, prevSteps.length - 1));
    }
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "500px", height: "800px", color: "white", boxShadow: 4, justifyContent: "center", alignItems: "center", gap: 2, paddingX: 3, children: [_jsx(Button, { disabled: isReceivingData || isSubmitting || !name || !sceneId, fullWidth: true, variant: "contained", onClick: handleSubmit, children: isSubmitting ? (_jsx(CircularProgress, {})) : laboratoryId == "-1" ? ("Add New Laboratory") : ("Update Laboratory") }), _jsx(TextField, { fullWidth: true, label: "Laboratory Name", value: name, onChange: handleNameChange }), _jsx(TextField, { fullWidth: true, type: "number", slotProps: { htmlInput: { min: 0 } }, label: "Scene ID", value: sceneId, onChange: handleSceneIdChange }), _jsxs(Box, { width: "100%", height: 560, overflow: "auto", children: [checkListStepList.map((step, index) => (_jsx(TextField, { fullWidth: true, label: `Checklist Step ${index + 1}`, value: step, onChange: (event) => handleCheckListStepList(event, index), sx: { marginY: 1 } }, index))), _jsxs(Stack, { direction: "row-reverse", children: [_jsx(IconButton, { children: _jsx(AddIcon, { onClick: addCheckListStep }) }), checkListStepList.length > 1 ? (_jsx(IconButton, { children: _jsx(RemoveIcon, { onClick: removeLastCheckListStep }) })) : (_jsx(_Fragment, {}))] })] })] }));
}
