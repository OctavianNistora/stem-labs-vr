import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { Box, Button, IconButton, Stack } from "@mui/material";
import { DataGrid, useGridApiRef } from "@mui/x-data-grid";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import RemoveCircleIcon from "@mui/icons-material/RemoveCircle";
import UpgradeIcon from "@mui/icons-material/Upgrade";
import { Link } from "react-router";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function ManageUsersPage() {
    const [rows, setRows] = useState([]);
    const apiRef = useGridApiRef();
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    useEffect(() => {
        if (!user) {
            return;
        }
        axiosRequestWithAutoReauth({
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/users`,
            headers: {
                Authorization: `Bearer ${user.accessToken}`,
            },
        }, setUser)
            .then((response) => {
            const rowsData = response.data.map((user) => ({
                uid: user.uid,
                firstName: user.firstName,
                lastName: user.lastName,
                phoneNumber: user.phoneNumber,
                email: user.email,
                createdAt: user.createdAt,
                role: user.role,
                isUpdating: false,
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
    function handleUpdateUserRole(uid, role) {
        setRows((prevRows) => prevRows.map((row) => row.uid === uid ? { ...row, isUpdating: true } : row));
        axiosRequestWithAutoReauth({
            method: "PUT",
            url: `${import.meta.env.VITE_API_URL}/api/users/${uid}/role`,
            data: role,
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${user?.accessToken}`,
            },
        }, setUser)
            .then((_) => {
            setRows((prevRows) => prevRows.map((row) => row.uid === uid ? { ...row, role: role } : row));
            addToast({
                message: `User with ID ${uid} now has role ${role}.`,
                variant: "success",
            });
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
        })
            .finally(() => {
            setRows((prevRows) => prevRows.map((row) => row.uid === uid ? { ...row, isUpdating: false } : row));
        });
    }
    function handleDeleteUser(uid) {
        setRows((prevRows) => prevRows.map((row) => row.uid === uid ? { ...row, isUpdating: true } : row));
        axiosRequestWithAutoReauth({
            method: "DELETE",
            url: `${import.meta.env.VITE_API_URL}/api/users/${uid}`,
            headers: {
                Authorization: `Bearer ${user?.accessToken}`,
            },
        }, setUser)
            .then((_) => {
            setRows((prevRows) => prevRows.filter((row) => row.uid !== uid));
            addToast({
                message: `User with ID ${uid} has been deleted.`,
                variant: "success",
            });
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
            setRows((prevRows) => prevRows.map((row) => row.uid === uid ? { ...row, isUpdating: false } : row));
        });
    }
    const columns = [
        {
            field: "uid",
            align: "center",
            headerName: "ID",
            headerAlign: "center",
            type: "number",
        },
        {
            field: "firstName",
            align: "center",
            headerName: "First Name",
            headerAlign: "center",
        },
        {
            field: "lastName",
            align: "center",
            headerName: "Last Name",
            headerAlign: "center",
        },
        {
            field: "phoneNumber",
            align: "center",
            headerName: "Phone Number",
            headerAlign: "center",
        },
        {
            field: "email",
            align: "center",
            headerName: "Email",
            headerAlign: "center",
        },
        {
            field: "createdAt",
            align: "center",
            headerName: "Created At",
            headerAlign: "center",
            type: "dateTime",
            valueGetter: (value) => new Date(value),
        },
        {
            field: "role",
            align: "center",
            headerName: "Role",
            headerAlign: "center",
        },
        {
            field: "actions",
            type: "actions",
            renderCell: (params) => {
                return (_jsxs(Stack, { direction: "row", justifyContent: "right", children: [params.row.role.toLowerCase() == "professor" ? (_jsx(IconButton, { title: "Demote to Student", onClick: () => handleUpdateUserRole(params.row.uid, "Student"), children: _jsx(UpgradeIcon, { color: "primary", sx: { transform: "rotate(180deg)" } }) })) : (_jsx(Box, { width: "40px" })), params.row.role.toLowerCase() == "student" ? (_jsx(IconButton, { title: "Promote to Professor", onClick: () => handleUpdateUserRole(params.row.uid, "Professor"), children: _jsx(UpgradeIcon, { color: "primary" }) })) : (_jsx(Box, { width: "40px" })), _jsx(IconButton, { title: "Delete User", onClick: () => handleDeleteUser(params.row.uid), children: _jsx(RemoveCircleIcon, { color: "error" }) })] }));
            },
        },
    ];
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "1400px", height: "800px", boxShadow: 4, children: [_jsx(Button, { component: Link, to: "/add-user", children: "Add New User" }), rows.length > 0 ? (_jsx(Box, { flexGrow: 1, children: _jsx(DataGrid, { apiRef: apiRef, rows: rows, getRowId: (row) => row.uid, columns: columns, disableRowSelectionOnClick: true, disableColumnResize: true, autoPageSize: true }) })) : (_jsx(_Fragment, {}))] }));
}
