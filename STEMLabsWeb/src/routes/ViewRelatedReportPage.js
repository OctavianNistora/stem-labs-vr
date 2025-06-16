import { jsx as _jsx, Fragment as _Fragment, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Button, Divider, IconButton, Stack, TextField, Typography, } from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { Link, useNavigate, useParams } from "react-router";
import { useContext, useEffect, useState } from "react";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import dateToFormattedString from "../helpers/dateToFormatedString.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function ViewRelatedReportPage() {
    const { sessionId, userId, reportId } = useParams();
    const [reportTabList, setReportTabList] = useState([]);
    const [reportDetailsList, setReportDetailsList] = useState([]);
    const navigate = useNavigate();
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    useEffect(() => {
        if (!isStringPositiveInteger(userId)) {
            navigate("..");
            return;
        }
        fetchReportsList();
    }, []);
    useEffect(() => {
        if (!reportId) {
            return;
        }
        if (!isStringPositiveInteger(reportId)) {
            navigate(`../${userId}`);
        }
        if (reportDetailsList
            .map((report) => report.id.toString())
            .some((id) => id === reportId)) {
            return;
        }
        fetchReportDetails();
    }, [reportId]);
    async function fetchReportsList() {
        try {
            const response = await axiosRequestWithAutoReauth({
                method: "GET",
                url: `${import.meta.env.VITE_API_URL}/api/laboratory-sessions/${sessionId}/participants/${userId}/reports`,
                headers: {
                    Authorization: `Bearer ${user?.accessToken}`,
                },
            }, setUser);
            setReportTabList(response.data
                .map((report) => ({
                id: report.id,
                date: new Date(report.submittedAt),
            }))
                .sort((a, b) => a.date.getTime() - b.date.getTime()));
        }
        catch (error) {
            toastErrorMessageHandle(addToast, setUser, error);
        }
    }
    async function fetchReportDetails() {
        try {
            const response = await axiosRequestWithAutoReauth({
                method: "GET",
                url: `${import.meta.env.VITE_API_URL}/api/laboratory-reports/${reportId}`,
                headers: {
                    Authorization: `Bearer ${user?.accessToken}`,
                },
            }, setUser);
            const reportDetails = {
                id: response.data.id,
                submitter: response.data.submitterFullName,
                steps: response.data.checklistSteps.map((step) => ({
                    statement: step.statement,
                    isCompleted: step.isCompleted,
                })),
                link: response.data.observationsImageLink,
            };
            setReportDetailsList((prev) => [...prev, reportDetails]);
        }
        catch (error) {
            toastErrorMessageHandle(addToast, setUser, error);
            navigate(`../${userId}`);
        }
    }
    const currentReportDetails = reportDetailsList.find((report) => report.id.toString() === reportId);
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "800px", height: "800px", bgcolor: "white", boxShadow: 4, overflow: "visible", alignItems: "center", children: [_jsxs(Box, { width: "100%", display: "flex", children: [_jsx(Box, { width: 0, height: 40, display: "flex", flexDirection: "row-reverse", children: _jsx(IconButton, { component: Link, to: user?.role.toLowerCase() === "student" ? "../.." : `..`, children: _jsx(ArrowBackIcon, {}) }) }), _jsxs(Box, { width: "100%", display: "flex", flexDirection: "column", children: [_jsx(Typography, { variant: "h4", marginY: 2, textAlign: "center", children: "Select a report to view" }), _jsx(Divider, { sx: { width: "100%" } }), _jsx(Box, { width: "100%", height: "100%", children: _jsx(Stack, { id: "reports-list", width: "100%", height: "100%", direction: "row", overflow: "auto", children: reportTabList.map((report, index) => (_jsxs(Box, { display: "flex", id: report.id.toString(), whiteSpace: "nowrap", children: [_jsx(Button, { component: Link, to: (reportId == undefined ? "" : `../${userId}/`) +
                                                    `${report.id}`, replace: true, children: _jsx(Typography, { color: "text.primary", fontWeight: report.id.toString() == reportId ? "bold" : "normal", children: dateToFormattedString(report.date) }) }), index < reportTabList.length - 1 ? (_jsx(Divider, { orientation: "vertical" })) : (_jsx(_Fragment, {}))] }, report.id))) }) })] })] }), _jsx(Divider, { sx: { width: "100%" } }), currentReportDetails ? (_jsxs(Box, { width: "100%", height: "100%", display: "flex", flexDirection: "column", padding: 2, overflow: "hidden", children: [_jsx(TextField, { id: "submitter-full-name", variant: "standard", label: "Submitter", value: currentReportDetails.submitter, slotProps: {
                            input: {
                                readOnly: true,
                                disableUnderline: true,
                            },
                            inputLabel: {
                                shrink: true,
                            },
                        } }), _jsxs(Box, { display: "flex", flexDirection: "column", overflow: "hidden", children: [_jsx(Stack, { direction: "column", overflow: "auto", children: currentReportDetails.steps.map((step, index) => (_jsx(TextField, { id: `step-${index + 1}`, variant: "standard", label: `Step ${index + 1}`, value: step.statement, multiline: true, color: step.isCompleted ? "success" : "error", focused: true, slotProps: {
                                        input: {
                                            readOnly: true,
                                            disableUnderline: true,
                                        },
                                        inputLabel: {
                                            shrink: true,
                                        },
                                    } }, currentReportDetails.id + "-" + index))) }), _jsx(Typography, { color: currentReportDetails.link ? "success" : "error", marginY: 1, children: currentReportDetails.link
                                    ? "Observations image:"
                                    : "No observations image" }), currentReportDetails.link ? (_jsx(Box, { display: "flex", height: "200px", children: _jsx(Box, { height: "100%", marginRight: "auto", boxShadow: 2, children: _jsx(Link, { to: currentReportDetails.link, target: "_blank", children: _jsx("img", { id: `observations-image`, src: currentReportDetails.link, alt: "Observations", style: {
                                                maxHeight: "100%",
                                                objectFit: "cover",
                                            } }) }) }) })) : (_jsx(_Fragment, {}))] })] })) : (_jsx(_Fragment, {}))] }));
}
