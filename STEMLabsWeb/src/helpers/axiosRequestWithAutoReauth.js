import axios, {} from "axios";
export async function axiosRequestWithAutoReauth(config, setUser) {
    try {
        return await axios(config);
    }
    catch (error) {
        if (axios.isAxiosError(error) && error.response?.status === 401) {
            try {
                const storageRefreshToken = localStorage.getItem("refreshToken");
                if (!storageRefreshToken) {
                    throw error;
                }
                const abortController = new AbortController();
                const refreshResponse = await axios.post(`${import.meta.env.VITE_API_URL}/api/auth/refresh-token`, storageRefreshToken, {
                    headers: {
                        "Content-Type": "application/json",
                    },
                    signal: abortController.signal,
                });
                const { uid, accessToken, refreshToken, role } = refreshResponse.data;
                setUser({ uid, accessToken, role });
                localStorage.setItem("refreshToken", refreshToken);
                const newConfig = {
                    ...config,
                    headers: {
                        ...config.headers,
                        Authorization: `Bearer ${accessToken}`,
                    },
                };
                return await axios(newConfig);
            }
            catch (refreshError) {
                throw refreshError;
            }
        }
        throw error;
    }
}
