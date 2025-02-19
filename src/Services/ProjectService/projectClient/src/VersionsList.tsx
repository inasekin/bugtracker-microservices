import * as React from "react" 
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { VersionRecord, columns } from "./projectTables/versionColumns"
import { DataTable } from "./projectTables/data-table"
 
function getData(): VersionRecord[] {
  return [
    {
      id: "1",
      version: "1.0",
      description: "Первая версия. Реализация микросервисов",
      command: "Редактировать",
    },
    {
      id: "2",
      version: "2.0",
      description: "Вторая версия. Добавлен фронт. Пересмотрена архитектура",
      command: "Редактировать",
    }
  ];
}
 
export function VersionsList() {

  const data = getData();

  return (
    <Card>
      <CardHeader>
        <CardTitle>Версии</CardTitle>
        <CardDescription className="my-2">Версии проекта используются для фиксации изменений. <br />
	При создании замечания указывается начальная версия начиная с которой необходимо исправить замечание.</CardDescription>
      </CardHeader>
      <CardContent>
 
    	<div className="container mx-auto ">
      		<DataTable columns={columns} data={data} /> 
	    </div>
      </CardContent>
      <CardFooter className="flex justify-between">
        <Button id="addUser">Добавить версию...</Button>
      </CardFooter>
    </Card>
  )
}