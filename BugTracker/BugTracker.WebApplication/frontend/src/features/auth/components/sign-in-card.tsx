"use client";
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card";
import {DottedSeparator} from "@/components/dotted-separator";
import { FaGithub } from "react-icons/fa";
import {Form, FormField, FormItem, FormControl, FormMessage} from "@/components/ui/form";
import {useForm} from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {zodResolver} from "@hookform/resolvers/zod";
import {loginSchema, LoginSchema} from "@/features/auth/schemas";
import Link from "next/link";
import { useLogin } from "../api/use-login";
import {signUpWithGithub} from "@/lib/github";

export const SignInCard = () => {
    const { mutate, isPending } = useLogin();
    const form = useForm<LoginSchema>({
        resolver: zodResolver(loginSchema),
        defaultValues: {
            email: "",
            password: "",
        },
    });

    const onSubmit = (values: LoginSchema) => {
        mutate(values);
    };

    return (
        <Card className="size-full md:w-[487px] border-none shadow-none">
            <CardHeader className="flexx items-center justify-center text-center p-7">
                <CardTitle className="text-2xl">С возвращением!</CardTitle>
            </CardHeader>
            <div className="px-7">
                <DottedSeparator/>
            </div>
            <CardContent className="p-7">
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                        <FormField
                            name="email"
                            control={form.control}
                            render={({field}) => (
                                <FormItem>
                                    <FormControl>
                                        <Input
                                            {...field}
                                            type="email"
                                            placeholder="Введите email"
                                        />
                                    </FormControl>
                                    <FormMessage/>
                                </FormItem>
                            )}
                        />
                        <FormField
                            name="password"
                            control={form.control}
                            render={({field}) => (
                                <FormItem>
                                    <FormControl>
                                        <Input
                                            {...field}
                                            type="password"
                                            placeholder="Введите пароль"
                                        />
                                    </FormControl>
                                    <FormMessage/>
                                </FormItem>
                            )}
                        />
                        <Button className="w-full" size="lg" disabled={isPending}>
                            Войти
                        </Button>
                    </form>
                </Form>
            </CardContent>
            <div className="px-7">
                <DottedSeparator/>
            </div>
            <CardContent className="p-7 flex flex-col gap-y-4">
                <Button
                    onClick={() => signUpWithGithub()}
                    disabled={isPending}
                    variant="secondary"
                    size="lg"
                    className="w-full"
                >
                    <FaGithub className="size-5 mr-2" />
                    Войти с помощью Github
                </Button>
            </CardContent>
            <div className="px-7">
                <DottedSeparator />
            </div>
            <CardContent className="p-7 flex items-center justify-center">
                <p>
                    Нет аккаунта?
                    <Link href="/sign-up">
                        <span className="text-blue-700">&nbsp;Зарегистрироваться</span>
                    </Link>
                </p>
            </CardContent>
        </Card>
    )
};