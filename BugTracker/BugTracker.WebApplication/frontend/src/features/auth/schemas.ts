import { z } from "zod";

export const loginSchema = z.object({
    email: z.string().email({ message: "Некорректный email" }),
    password: z.string().min(1, { message: "Обязательное поле" }),
});
export const registerSchema = z.object({
    name: z.string().trim().min(1, { message: "Обязательное поле" }),
    email: z.string().email({ message: "Некорректный email" }),
    password: z.string().min(8, { message: "Минимум 8 символов" }),
});

export type LoginSchema = z.infer<typeof loginSchema>;
export type RegisterSchema = z.infer<typeof registerSchema>;