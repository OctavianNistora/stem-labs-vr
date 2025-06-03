import axios, { type AxiosRequestConfig, type AxiosResponse } from "axios";
import type { Dispatch, SetStateAction } from "react";
import type { User } from "../contexts/AuthContext.tsx";

export async function axiosRequestWithAutoReauth<
  T = any,
  R = AxiosResponse<T>,
  D = any,
>(
  config: AxiosRequestConfig<D>,
  setUser: Dispatch<SetStateAction<User | null>>,
): Promise<R> {
  try {
    return await axios<T, R, D>(config);
  } catch (error) {
    if (axios.isAxiosError<T, D>(error) && error.response?.status === 401) {
      try {
        const storageRefreshToken = localStorage.getItem("refreshToken");
        if (!storageRefreshToken) {
          throw error;
        }

        const abortController = new AbortController();
        const refreshResponse = await axios.post(
          `${import.meta.env.VITE_API_URL}/api/auth/refresh-token`,
          storageRefreshToken,
          {
            headers: {
              "Content-Type": "application/json",
            },
            signal: abortController.signal,
          },
        );

        const { uid, accessToken, refreshToken, role } = refreshResponse.data;
        setUser({ uid, accessToken, role });
        localStorage.setItem("refreshToken", refreshToken);

        const newConfig: AxiosRequestConfig<D> = {
          ...config,
          headers: {
            ...config.headers,
            Authorization: `Bearer ${accessToken}`,
          },
        };
        return await axios<T, R, D>(newConfig);
      } catch (refreshError) {
        throw refreshError;
      }
    }

    throw error;
  }
}
