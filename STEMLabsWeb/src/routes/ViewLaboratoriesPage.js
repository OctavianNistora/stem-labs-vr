import { jsx as _jsx, Fragment as _Fragment, jsxs as _jsxs } from "react/jsx-runtime";
import { DataGrid, useGridApiRef, } from "@mui/x-data-grid";
import { Box, Button } from "@mui/material";
import { Link, useNavigate } from "react-router";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
const columns = [
    {
        field: "id",
        align: "center",
        headerName: "ID",
        headerAlign: "center",
        type: "number",
    },
    {
        field: "name",
        align: "center",
        headerName: "Laboratory Name",
        headerAlign: "center",
    },
    {
        field: "sceneId",
        align: "center",
        headerName: "Scene ID",
        headerAlign: "center",
    },
    {
        field: "checkListStepCount",
        align: "center",
        headerName: "No. of Checklist Steps",
        headerAlign: "center",
    },
];
export default function ViewLaboratoriesPage() {
    const [rows, setRows] = useState([]);
    const apiRef = useGridApiRef();
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    const navigate = useNavigate();
    useEffect(() => {
        axiosRequestWithAutoReauth({
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/laboratories`,
            headers: {
                Authorization: `Bearer ${user?.accessToken}`,
            },
        }, setUser)
            .then((response) => {
            const rowsData = response.data.map((lab) => ({
                id: lab.id,
                name: lab.name,
                sceneId: lab.sceneId,
                checkListStepCount: lab.checkListStepCount,
            }));
            setRows(rowsData);
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
        });
    }, []);
    useEffect(() => {
        apiRef?.current?.autosizeColumns({
            includeHeaders: true,
            includeOutliers: true,
            expand: true,
        });
    });
    function handleRowClick(params) {
        navigate(`${params.id}`);
    }
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "1400px", height: "800px", boxShadow: 4, children: [_jsx(Button, { component: Link, to: "-1", children: "Add New Laboratory" }), rows.length > 0 ? (_jsx(Box, { flexGrow: 1, children: _jsx(DataGrid, { apiRef: apiRef, rows: rows, columns: columns, disableRowSelectionOnClick: true, disableColumnResize: true, autoPageSize: true, onRowClick: handleRowClick }) })) : (_jsx(_Fragment, {}))] }));
}
